using MailKit.Security;
using MimeKit;
using Services.Configurations;
using Services.Implementations;
using Tests.Helpers;

namespace Tests.TestServices;

public class MailPackerTests
{
   private static readonly string SenderMail = Env.GetVariable("Email:Sender:mail");
   private static readonly string ReceiverMail = Env.GetVariable("Email:Receiver:mail");
   private static readonly string SenderName = Env.GetVariable("Email:Sender:username");
   private static readonly string ReceiverName = Env.GetVariable("Email:Receiver:username");
   
   private readonly SmtpServerConfiguration _smtpConfig = new SmtpServerConfiguration()
   {
      Alias = Env.GetVariable("Email:Alias"),
      Host = Env.GetVariable("Email:Host"),
      Key = Env.GetVariable("Email:Key"),
      Port = int.Parse(Env.GetVariable("Email:Port")),
      SecureSocketOptions = SecureSocketOptions.StartTls,
      UserHost = Env.GetVariable("Email:UserHost"),
   };
   
   private readonly MimeMessage _mailMessage = new MimeMessage()
   {
      Subject = "Test Subject",
      Body = new TextPart("Test Body Mail"),
      From = { new MailboxAddress(SenderMail, SenderName) },
      To = { new MailboxAddress(ReceiverMail, ReceiverName) }
   };
   
   
   [Fact]
   public async Task WhenSmtpConfigIsUnset()
   {
      //Arrange
      var service = new MailPack();
      
      //Act & Assert
      await Assert.ThrowsAsync<NullReferenceException>(async () =>
      {
         await service
            .SetSmtpConfig(null!)
            .SetMailMessage(_mailMessage)
            .SendAsync();
      });
   }
   
   [Fact]
   public async Task WhenMailMessageIsUnset()
   {
      //Arrange
      var service = new MailPack();
      
      //Act & Assert
      await Assert.ThrowsAsync<NullReferenceException>(async () =>
      {
         await service
            .SetSmtpConfig(_smtpConfig)
            .SetMailMessage(null!)
            .SendAsync();
      });
   }
}