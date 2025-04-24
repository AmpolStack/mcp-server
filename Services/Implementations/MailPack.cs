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

    public void Clear()
    {
        _mailMessage = null;
    }
    
    public IMailPacker SetSmtpConfig(SmtpServerConfiguration config)
    {
        _smtpConfig = config;
        return this;
    }

    public IMailPacker SetMailMessage(MimeMessage message)
    {
        _mailMessage = message;
        return this;
    }
    
    public async Task SendAsync()
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpConfig!.Host, _smtpConfig.Port, _smtpConfig!.SecureSocketOptions);
        await client.AuthenticateAsync(_smtpConfig.UserHost, _smtpConfig.Key);
        await client.SendAsync(_mailMessage);
        await client.DisconnectAsync(true);
        Clear();
    }
    
}