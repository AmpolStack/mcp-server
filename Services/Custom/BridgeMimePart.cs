using MimeKit;

namespace Services.Custom;

public class BridgeMimePart
{
    public string? Path { get; set; }
    public string? MediaType { get; set; }
    public string? SubType = "application";
    public ContentEncoding Encoding = ContentEncoding.Base64; 
}