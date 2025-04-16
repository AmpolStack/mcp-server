using MimeKit;
using Services.Configurations;

namespace Services.Definitions;

public interface IEmailService
{
    public IEmailService SetSenderEmail(string username, string address);
    public IEmailService AddReceiverAddress(string username, string address);
    public IEmailService SetMessageBody(string content);
    public IEmailService SetMessageSubject(string content);
    public IEmailService SetMessage(string subject, string body);
    public IEmailService AddFile(string path, string subType);
    public IEmailService AddFile(string path, string mediaType, string subType, ContentEncoding encoding);
    public Task<IMailPacker> BuildAsync();
   
}
