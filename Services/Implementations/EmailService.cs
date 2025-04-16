using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Custom;
using Services.Definitions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Services.Implementations;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private MailBox _mailbox = new MailBox();
    private readonly IMailPacker _mailPacker;

    private void Reset()
    {
        _mailbox = new MailBox();
    }

    private MimePart TransformBridgeToMime(BridgeMimePart item)
    {
        using var stream = File.OpenRead(item.Path!);
        var attachment = new MimePart(item.MediaType, item.SubType)
        {
            Content = new MimeContent(stream, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(item.Path)
        };
        return attachment;
    }
    private bool ResetAndReturnFalse(string message)
    {
        Reset();
        _logger.LogError("error: {message}", message);
        return false;
    }

    public void SetSenderEmail(string username, string address)
    {
        _mailbox.Sender = new MailboxAddress(Encoding.Unicode, username , address);
    }

    public void AddReceiverAddress(string username, string address)
    {
        _mailbox.Recipients.Add(new MailboxAddress(Encoding.Unicode, username , address));
    }
    public void SetMessageBody(string content)
    {
        _mailbox.Body = content;
    }
    public void SetMessageSubject(string content)
    {
        _mailbox.Subject = content;
    }

    public void SetMessage(string subject, string body)
    {
        _mailbox.Body = body;
        _mailbox.Subject = subject;
    }

    public void AddFile(string path, string subType)
    {
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Path = path,
            SubType = subType
        });
    }
    
    public void AddFile(string path, string mediaType, string subType, ContentEncoding encoding)
    {
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Encoding = encoding,
            Path = path,
            MediaType = mediaType,
            SubType = subType
        });
    }
    
    
    public EmailService(ILoggerFactory loggerFactory, IMailPacker mailPacker)
    {
        Reset();
        _logger = loggerFactory.CreateLogger<EmailService>();
        _mailPacker = mailPacker;
    }
    
    public async Task<IMailPacker> BuildAsync()
    {
        if (_mailbox.Sender == null)
        {
            ResetAndReturnFalse("The sender email address is null, its necessary.");
        }

        if (_mailbox.Recipients.Count == 0)
        {
            ResetAndReturnFalse("The recipient email address is null, its necessary.");
        }

        if (string.IsNullOrEmpty(_mailbox.Body))
        {
            ResetAndReturnFalse("The body email address is null, its necessary.");
        }

        var generatedId = Guid.NewGuid().ToString();
        if (string.IsNullOrEmpty(_mailbox.Subject))
        {
            _mailbox.Subject = "GENERATED EMAIL: " + generatedId;
        }

        var message = new MimeMessage();
        var tasks = new List<Task>();
        var multipart = new Multipart("mixed");
        
        var bodyText = new TextPart("plain")
        {
            Text = _mailbox.Body
        };
   
        if (_mailbox.BridgeFiles.Count > 0)
        {
            foreach (var item in _mailbox.BridgeFiles)
            {
                if (item.Path == null)
                {
                    ResetAndReturnFalse("One file path is null, its necessary.");
                }
                
                if (item.MediaType == null)
                {
                    //var fileType = Path.GetExtension(item.Path);
                    item.MediaType = "application";
                }
                var task = Task.Run(() =>
                {
                    var attachment = TransformBridgeToMime(item);
                    multipart.Add(attachment);
                });
                tasks.Add(task);
                
            }
            
        }
        
        message.From.Add(_mailbox.Sender);
        message.To.AddRange(_mailbox.Recipients);
        message.Subject = _mailbox.Subject;
        
        multipart.Add(bodyText);

        await Task.WhenAll(tasks);

        message.Body = multipart;
        Reset();
        return Pack(message);
    }

    private IMailPacker Pack(MimeMessage message)
    {
        _mailPacker.SetMailMessage(message);
        return _mailPacker;
    }
}

public class MailPack : IMailPacker
{
    private MimeMessage? _mailMessage;
    private SmtpServerConfiguration? _smtpConfig;
    private readonly ILogger<MailPack> _logger;

    public void Clear()
    {
        _mailMessage = null;
    }
    public MailPack(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<MailPack>();
    }
    public void SetSmtpConfig(SmtpServerConfiguration config)
    {
        _smtpConfig = config;
    }

    public void SetMailMessage(MimeMessage message)
    {
        _mailMessage = message;
    }
    
    public async Task<bool> SendAsync()
    {

        if (_smtpConfig == null)
        {
            _logger.LogError("no smtp config set");
            Clear();
            return false;
        }

        if (_mailMessage == null)
        {
            _logger.LogError("no mail message in queue");
            Clear();
            return false;
        }
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpConfig!.Host, _smtpConfig.Port, _smtpConfig!.SecureSocketOptions);
            await client.AuthenticateAsync(_smtpConfig.UserHost, _smtpConfig.Key);
            await client.SendAsync(_mailMessage);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            Clear();
            return false;
        }
    }
    
}
