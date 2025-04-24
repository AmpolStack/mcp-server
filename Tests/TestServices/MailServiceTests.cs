using MailKit;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Definitions;
using Services.Implementations;
using Tests.Helpers;

namespace Tests.TestServices;

public class MailServiceTests
{
    private readonly Mock<IMailPacker> _mailPacker;
    private static readonly string SenderMail = Env.GetVariable("Email:Sender:mail");
    private static readonly string ReceiverMail = Env.GetVariable("Email:Receiver:mail");
    private static readonly string SenderName = Env.GetVariable("Email:Sender:username");
    private static readonly string ReceiverName = Env.GetVariable("Email:Receiver:username");

    public MailServiceTests()
    {
        var packer = new Mock<IMailPacker>();
        _mailPacker = packer;
    }

    [Fact]
    public void WhenSubjectOrBodyIsNull()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);

        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.SetMessage(null!, null!));
    }

    [Fact]
    public void WhenSenderIsNull()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.SetSenderEmail(null!, null!));
    }

    [Fact]
    public void WhenReceiverIsNull()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.AddReceiverAddress(null!, null!));
    }

    [Fact]
    public void WhenIsNotNull()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.AddFile(null!, null!));
    }

    [Fact]
    public async Task WhenBuildAndTheReceiverIsNull()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);
        
        //Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.SetMessage("Test Subject", "Test Body")
                    .SetSenderEmail(SenderName, SenderMail)
                    .BuildAsync()
            );
    }

    [Fact]
    public async Task WhenOneOrManyFilesNotExist()
    {
        //Arrange
        var packer = _mailPacker!;
        var service = new EmailService(packer.Object);
        
        //Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await service.SetMessage("Test Subject", "Test Body")
                .SetSenderEmail(SenderName, SenderMail)
                .AddReceiverAddress(ReceiverName, ReceiverMail)
                .AddFile("none", "none")
                .BuildAsync()
        );
    }
}