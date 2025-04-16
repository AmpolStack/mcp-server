using MimeKit;
using Services.Configurations;

namespace Services.Definitions;


public interface IMailPacker
{
    public IMailPacker SetSmtpConfig(SmtpServerConfiguration config);
    public IMailPacker SetMailMessage(MimeMessage message);
    public void Clear();
    public Task<bool> SendAsync();
}