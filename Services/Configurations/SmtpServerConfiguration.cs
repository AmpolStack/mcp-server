using MailKit.Security;

namespace Services.Configurations;

public class SmtpServerConfiguration
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? UserHost  { get; set; }
    public string? Alias  { get; set; }
    public string? Key { get; set; }
    public SecureSocketOptions SecureSocketOptions { get; set; }
}