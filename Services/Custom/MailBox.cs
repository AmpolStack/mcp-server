using MimeKit;

namespace Services.Custom;

public class MailBox
{
    public MailboxAddress? Sender { get; set; }
    public List<MailboxAddress> Recipients = [];
    public List<BridgeMimePart> BridgeFiles = [];
    public string? Subject { get; set; }
    public string? Body { get; set; }
}