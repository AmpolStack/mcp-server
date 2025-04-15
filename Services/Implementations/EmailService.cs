using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Custom;
using Services.Definitions;

namespace Services.Implementations;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private MailBox _mailbox = new MailBox();

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
    private bool ResetAndReturn(string message)
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
    
    
    public EmailService(ILoggerFactory loggerFactory)
    {
        Reset();
        _logger = loggerFactory.CreateLogger<EmailService>();
    }
    
    //TODO: Add other layer (message sender layer) for most extensibility in service, but preserves this method
    public async Task<bool> BuildAndSendAsync(SmtpServerConfiguration config)
    {
        if (_mailbox.Sender == null)
        {
            return ResetAndReturn("The sender email address is null, its necessary.");
        }

        if (_mailbox.Recipients.Count == 0)
        {
            return ResetAndReturn("The recipient email address is null, its necessary.");
        }

        if (string.IsNullOrEmpty(_mailbox.Body))
        {
            return ResetAndReturn("The body email address is null, its necessary.");
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
                    return ResetAndReturn("One file path is null, its necessary.");
                }
                
                if (item.MediaType == null)
                {
                    var fileType = Path.GetExtension(item.Path);
                    item.MediaType = fileType;
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
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(config.Host, config.Port, config.SecureSocketOptions);
            await client.AuthenticateAsync(config.UserHost, config.Key);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
    
}