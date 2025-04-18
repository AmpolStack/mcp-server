﻿using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using MimeKit;
using Services.Configurations;
using Services.Custom;
using Services.Definitions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Services.Implementations;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private MailBox _mailbox = new MailBox();
    private readonly IMailPacker _mailPacker;

    private void Reset()
    {
        _mailbox = new MailBox();
    }

    private async Task<MimePart> TransformBridgeToMime(BridgeMimePart item)
    {
        var fileBytes = await File.ReadAllBytesAsync(item.Path!);
        var memoryStream = new MemoryStream(fileBytes);
        var attachment = new MimePart(item.MediaType, item.SubType)
        {
            Content = new MimeContent(memoryStream, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(item.Path)
        };
        return attachment;
    }
    private void ResetAndLogError(string message)
    {
        Reset();
        _logger.LogError("error: {message}", message);
    }

    public IEmailService SetSenderEmail(string username, string address)
    {
        _mailbox.Sender = new MailboxAddress(Encoding.Unicode, username , address);
        return this;
    }

    public IEmailService AddReceiverAddress(string username, string address)
    {
        _mailbox.Recipients.Add(new MailboxAddress(Encoding.Unicode, username , address));
        return this;
    }
    public IEmailService SetMessageBody(string content)
    {
        _mailbox.Body = content;
        return this;
    }
    public IEmailService SetMessageSubject(string content)
    {
        _mailbox.Subject = content;
        return this;
    }

    public IEmailService SetMessage(string subject, string body)
    {
        _mailbox.Body = body;
        _mailbox.Subject = subject;
        return this;
    }

    public IEmailService AddFile(string path, string subType)
    {
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Path = path,
            SubType = subType
        });
        return this;
    }
    
    public IEmailService AddFile(string path, string mediaType, string subType, ContentEncoding encoding)
    {
        _mailbox.BridgeFiles.Add(new BridgeMimePart()
        {
            Encoding = encoding,
            Path = path,
            MediaType = mediaType,
            SubType = subType
        });
        return this;
    }
    
    
    public EmailService(ILoggerFactory loggerFactory, IMailPacker mailPacker)
    {
        Reset();
        _logger = loggerFactory.CreateLogger<EmailService>();
        _mailPacker = mailPacker;
    }
    
    public async Task<IMailPacker> BuildAsync()
    {
        if (_mailbox.Sender == null)
        {
            ResetAndLogError("The sender email address is null, its necessary.");
        }

        if (_mailbox.Recipients.Count == 0)
        {
            ResetAndLogError("The recipient email address is null, its necessary.");
        }

        if (string.IsNullOrEmpty(_mailbox.Body))
        {
            ResetAndLogError("The body email address is null, its necessary.");
        }

        var generatedId = Guid.NewGuid().ToString();
        if (string.IsNullOrEmpty(_mailbox.Subject))
        {
            _mailbox.Subject = "GENERATED EMAIL: " + generatedId;
        }

        var message = new MimeMessage();
        var tasks = new List<Task>();
        var multipart = new Multipart("mixed");
        
        var bodyText = new TextPart("plain")
        {
            Text = _mailbox.Body
        };
   
        if (_mailbox.BridgeFiles.Count > 0)
        {
            foreach (var item in _mailbox.BridgeFiles)
            {
                if (item.Path == null)
                {
                    ResetAndLogError("One file path is null, its necessary.");
                }
                
                if (item.MediaType == null)
                {
                    //var fileType = Path.GetExtension(item.Path);
                    item.MediaType = "application";
                }
                var task = Task.Run(async () =>
                {
                    var attachment = await TransformBridgeToMime(item);
                    multipart.Add(attachment);
                });
                tasks.Add(task);
                
            }
            
        }
        
        message.From.Add(_mailbox.Sender);
        message.To.AddRange(_mailbox.Recipients);
        message.Subject = _mailbox.Subject;
        
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

