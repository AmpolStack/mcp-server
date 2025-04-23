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
    public void WhenSubjectOrBodyIsNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var packer = _mailPacker!;
        var service = new EmailService(logger.Object, packer.Object);

        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.SetMessage(null!, null!));
        _loggerFactory.Verify(x => x.CreateLogger(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    public void WhenSenderIsNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var packer = _mailPacker!;
        var service = new EmailService(logger.Object, packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.SetSenderEmail(null!, null!));
        _loggerFactory.Verify(x => x.CreateLogger(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void WhenReceiverIsNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var packer = _mailPacker!;
        var service = new EmailService(logger.Object, packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.AddReceiverAddress(null!, null!));
        _loggerFactory.Verify(x => x.CreateLogger(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void WhenIsNotNull()
    {
        //Arrange
        var logger = _loggerFactory!;
        var packer = _mailPacker!;
        var service = new EmailService(logger.Object, packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.AddFile(null!, null!));
        _loggerFactory.Verify(x => x.CreateLogger(It.IsAny<string>()), Times.Once);
    }
    
}