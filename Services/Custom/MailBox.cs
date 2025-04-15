using MimeKit;

namespace Services.Custom;

public class MailBox
{
    public MailboxAddress? Sender { get; set; }
    public List<MailboxAddress>? Recipients { get; set; }
    public List<(string fileType, string filePath)>? Files { get; set; }
    public string? Message { get; set; }
    public string? Body { get; set; }
}