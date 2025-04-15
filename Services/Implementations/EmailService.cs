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
    
    //TODO: This file will replaced "SendMailAsync" method
    public Task<bool> BuildAndSendAsync()
    {
        throw new NotImplementedException();
    }
    
    //TODO: Delete this file
    public async Task<bool> SendEmailAsync(string subject, string body, string toName, string toAddress, string path, SmtpServerConfiguration config)
    {
        var receiverAddress = new MailboxAddress(name: toName, address: toAddress);
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(config.Alias, config.UserHost));
        message.To.Add(receiverAddress);
        message.Subject = subject;

        await using var stream = File.OpenRead(path);
        var attachments = new MimePart("application", "pdf")
        {
            Content = new MimeContent(stream, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(path)
        };
        
        var bodyText = new TextPart("plain")
        {
            Text = body
        };
        
        var multipart = new Multipart("mixed");
        multipart.Add(bodyText);
        multipart.Add(attachments);
        
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