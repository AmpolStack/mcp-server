using MimeKit;
using Services.Configurations;

namespace Services.Definitions;

public interface IEmailService
{
    public void SetSenderEmail(string username, string address);
    public void AddReceiverAddress(string username, string address);
    public void SetMessageBody(string content);
    public void SetMessageSubject(string content);
    public void SetMessage(string subject, string body);
    public void AddFile(string path, string subType);
    public void AddFile(string path, string mediaType, string subType, ContentEncoding encoding);
    public Task<bool> BuildAndSendAsync(SmtpServerConfiguration config);
   
}