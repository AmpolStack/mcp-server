﻿using System.Text;
using MimeKit;
using Services.Custom;
using Services.Definitions;

namespace Services.Implementations;

public class EmailService : IEmailService
{
    private MailBox _mailbox = new MailBox();
    private readonly IMailPacker _mailPacker;

    private void Reset()
    {
        _mailbox = new MailBox();
    }

    private void ProveIfNullOrEmpty(string param)
    {
        if (!string.IsNullOrEmpty(param)) return;
        Reset(); 
        throw new ArgumentNullException(nameof(param), nameof(param) + " cannot be null.");
    }
    
    private static async Task<MimePart> TransformBridgeToMime(BridgeMimePart item)
    {
        var fileBytes = await File.ReadAllBytesAsync(item.Path!);
        var memoryStream = new MemoryStream(fileBytes);
        var attachment = new MimePart(item.MediaType, item.SubType)
        {
            Content = new MimeContent(memoryStream),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(item.Path)
        };
        return attachment;
    }
  
    public IEmailService SetSenderEmail(string username, string address)
    {
        ProveIfNullOrEmpty(username);
        ProveIfNullOrEmpty(address);
        _mailbox.Sender = new MailboxAddress(Encoding.Unicode, username , address);
        return this;
    }

    public IEmailService AddReceiverAddress(string username, string address)
    {
        ProveIfNullOrEmpty(username);
        ProveIfNullOrEmpty(address);
        _mailbox.Recipients.Add(new MailboxAddress(Encoding.Unicode, username , address));
        return this;
    }
    public IEmailService SetMessageBody(string content)
    {
        ProveIfNullOrEmpty(content);
        _mailbox.Body = content;
        return this;
    }
    public IEmailService SetMessageSubject(string content)
    {
        ProveIfNullOrEmpty(content);
        _mailbox.Subject = content;
        return this;
    }

    public IEmailService SetMessage(string subject, string body)
    {
        SetMessageSubject(subject);
        SetMessageBody(body);
        return this;
    }

    public IEmailService AddFile(string path, string subType)
    {
        ProveIfNullOrEmpty(path);
        ProveIfNullOrEmpty(subType);
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Path = path,
            SubType = subType
        });
        return this;
    }
    
    public IEmailService AddFile(string path, string mediaType, string subType, ContentEncoding encoding)
    {
        ProveIfNullOrEmpty(path);
        ProveIfNullOrEmpty(subType);
        ProveIfNullOrEmpty(mediaType);
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Encoding = encoding,
            Path = path,
            MediaType = mediaType,
            SubType = subType
        });
        return this;
    }
    
    
    public EmailService(IMailPacker mailPacker)
    {
        Reset();
        _mailPacker = mailPacker;
    }
    
    public async Task<IMailPacker> BuildAsync()
    {
        
        var message = new MimeMessage();
        var tasks = new List<Task>();
        var multipart = new Multipart("mixed");
        
        message.From.Add(_mailbox.Sender);

        if (_mailbox.Recipients.Count <= 0)
        {
            throw new InvalidOperationException("No recipients have been provided.");
        }
        
        message.To.AddRange(_mailbox.Recipients);
        message.Subject = _mailbox.Subject;
        var bodyText = new TextPart("plain")
        {
            Text = _mailbox.Body
        };
        
        
        if (_mailbox.BridgeFiles.Count > 0)
        {
            foreach (var item in _mailbox.BridgeFiles)
            {
               
                ProveIfNullOrEmpty(item.Path!);
                
                item.MediaType ??= "application";
                
                var task = Task.Run(async () =>
                {
                    var attachment = await TransformBridgeToMime(item);
                    multipart.Add(attachment);
                });
                tasks.Add(task);
                
            }
            
        }
        
        multipart.Add(bodyText);

        await Task.WhenAll(tasks);

        message.Body = multipart;
        Reset();
        return Pack(message);
    }

    private IMailPacker Pack(MimeMessage message)
    {
        _mailPacker.SetMailMessage(message);
        return _mailPacker;
    }
}

