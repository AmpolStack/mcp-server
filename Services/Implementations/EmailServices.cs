using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Definitions;

namespace Services.Implementations;

public class EmailServices : IEmailService
{
    private ILogger<EmailServices> _logger;

    public EmailServices(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmailServices>();
    }
    
    public async Task<bool> SendEmailAsync(string subject, string body, string toName, string toAddress, SmtpServerConfiguration config)
    {
        var receiverAddress = new MailboxAddress(name: toName, address: toAddress);
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(config.Alias, config.UserHost));
        message.To.Add(receiverAddress);
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

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