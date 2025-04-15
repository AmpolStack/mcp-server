using System.Net.Mail;
using Services.Configurations;

namespace Services.Custom;

public class MailPack
{
    public required MailMessage MailMessage { get; set; }
    public required SmtpServerConfiguration SmtpConfig { get; set; }
}