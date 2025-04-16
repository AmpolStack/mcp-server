using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Definitions;

namespace Services.Implementations;

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