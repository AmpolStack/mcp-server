using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Definitions;

namespace Services.Implementations;

public class EmailService : IEmailService
{
    private ILogger<EmailService> _logger;

    public EmailService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmailService>();
    }
    
    //TODO: Implements the builder pattern
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