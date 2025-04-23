using MailKit;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Definitions;
using Services.Implementations;
using Tests.Helpers;

namespace Tests.TestServices;

public class MailServiceTests
{
    private readonly Mock<ILoggerFactory> _loggerFactory;
    private readonly Mock<IMailPacker> _mailPacker;
    private static readonly string SenderMail = Env.GetVariable("Email:SenderMail:mail");
    private static readonly string ReceiverMail = Env.GetVariable("Email:ReceiverMail:mail");
    private static readonly string SenderName = Env.GetVariable("Email:SenderMail:username");
    private static readonly string ReceiverName = Env.GetVariable("Email:ReceiverMail:username");

    public MailServiceTests()
    {
        var factory = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger<EmailService>>();
        var packer = new Mock<IMailPacker>();
        
        factory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        
        _loggerFactory = factory;
        _mailPacker = packer;
        
    }

    [Fact]
    public async Task WhenSubjectOrBodyIsNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var packer = _mailPacker!;
        var service = new EmailService(logger.Object, packer.Object);

        
        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await service.SetMessage(null!, null!)
                .SetSenderEmail(SenderName, SenderMail)
                .AddReceiverAddress(ReceiverName, ReceiverMail)
                .BuildAsync()
        );
        
        _loggerFactory.Verify(x => x.CreateLogger(It.IsAny<string>()), Times.Once);

    }
    
    
    
}