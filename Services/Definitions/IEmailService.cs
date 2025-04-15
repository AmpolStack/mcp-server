using Services.Configurations;

namespace Services.Definitions;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(string subject, string body, string toName, string toAddress, SmtpServerConfiguration config);
}