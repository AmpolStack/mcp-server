using MimeKit;
using Services.Configurations;

namespace Services.Definitions;


public interface IMailPacker
{
    public void SetSmtpConfig(SmtpServerConfiguration config);
    public void SetMailMessage(MimeMessage message);
    public void Clear();
    public Task<bool> SendAsync();
}