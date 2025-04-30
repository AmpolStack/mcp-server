using MailKit.Security;
using McpServerly.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Services.Configurations;
using Services.Definitions;
using Services.Implementations;
using Tests.Helpers;
using ZstdSharp.Unsafe;

namespace Tests.ToolTest;

public class ReportToolsTest
{
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<ILogger<ReportsTool>> _logger;
    private readonly Mock<IHtmlGeneratorService> _htmlGeneratorService;
    private readonly Mock<IPdfGeneratorService> _pdfGeneratorService;
    private readonly Mock<IOptions<SmtpServerConfiguration>> _smtpConfig;
    private readonly Mock<IOptions<ResourceFiles>> _resourceFiles;
    
    public ReportToolsTest()
    {
        _emailService = new Mock<IEmailService>();
        _logger = new Mock<ILogger<ReportsTool>>();
        _htmlGeneratorService = new Mock<IHtmlGeneratorService>();
        _pdfGeneratorService = new Mock<IPdfGeneratorService>();
        _smtpConfig = new Mock<IOptions<SmtpServerConfiguration>>();
        _resourceFiles = new Mock<IOptions<ResourceFiles>>();
    }

    [Fact]
    public async Task WhenFilePathIsNull()
    {
        //Arrange
        _resourceFiles
            .SetupGet(p => p.Value)
            .Returns(new ResourceFiles(){ FilePath = null});
        
        var service = new ReportsTool(_emailService.Object,
            _logger.Object,
            _htmlGeneratorService.Object,
            _pdfGeneratorService.Object,
            _smtpConfig.Object,
            _resourceFiles.Object);
        
        //Act 
        var resp = await service.SendReportWithHtml("test subject", "<h1>test body</h1>");
        var resp2 = await service.SendReportWithHtml("test subject", "# test body");
        
        //Assert
        Assert.False(resp);
        Assert.False(resp2);
        
    }

    [Fact]
    public async Task WhenThePdfConversionFails()
    {
        //Arrange
        _resourceFiles
            .SetupGet(p => p.Value)
            .Returns(new ResourceFiles(){ FilePath = "temp/path"});
        
        _pdfGeneratorService
            .Setup(x => x.ConvertHtmlStringToPdf(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<Exception>();
        
        _htmlGeneratorService
            .Setup(x => x.GenerateFromMarkdownString(It.IsAny<string>()))
            .Returns("<h1>test body</h1>");
        
        var service = new ReportsTool(_emailService.Object,
            _logger.Object,
            _htmlGeneratorService.Object,
            _pdfGeneratorService.Object,
            _smtpConfig.Object,
            _resourceFiles.Object);
        
        //Act
        var resp = await service.SendReportWithHtml("test subject", "# test body");
        var resp2 = await service.SendReportWithMarkdown("test subject", "# test body");
        
        //Assert
        Assert.False(resp);
        Assert.False(resp2);
        _resourceFiles.VerifyGet(x => x.Value, Times.Once);
        _pdfGeneratorService.Verify(x => x.ConvertHtmlStringToPdf(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _htmlGeneratorService.Verify(x => x.GenerateFromMarkdownString(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task WhenTheMailSenderFails()
    {
        //Arrange
        var mailPackerMock = new Mock<IMailPacker>();
        
        _smtpConfig.SetupGet(x => x.Value)
            .Returns(new SmtpServerConfiguration()
            {
                Alias = "test",
                UserHost = "test@test.com",
                Host = "test@test2.com",
                Port = 123,
                SecureSocketOptions = SecureSocketOptions.None,
            });
        
        _resourceFiles
            .SetupGet(p => p.Value)
            .Returns(new ResourceFiles(){ FilePath = "temp/path"});

        _pdfGeneratorService
            .Setup(x => x.ConvertHtmlStringToPdf(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(await Task.FromResult("temp/path/file/generated"));

        _emailService
            .Setup(x => x
                .SetMessageSubject(It.IsAny<string>())
            )
            .Returns(_emailService.Object);
        
        _emailService
            .Setup(x => x
                .SetMessageBody(It.IsAny<string>())
            )
            .Returns(_emailService.Object);
        
        _emailService
            .Setup(x => x
                .SetSenderEmail(It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(_emailService.Object);
        
        _emailService
            .Setup(x => x
                .AddReceiverAddress(It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(_emailService.Object);
        
        _emailService
            .Setup(x => x
                .AddFile(It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(_emailService.Object);
        
        _emailService
            .Setup(x => x.BuildAsync())
            .ReturnsAsync(mailPackerMock.Object);
        
        var service = new ReportsTool(_emailService.Object,
            _logger.Object,
            _htmlGeneratorService.Object,
            _pdfGeneratorService.Object,
            _smtpConfig.Object,
            _resourceFiles.Object);

        //Act
        var resp = await service.SendReportWithHtml("test subject", "# test body");
        var resp2 = await service.SendReportWithMarkdown("test subject", "# test body");
        
        //Assert
        Assert.False(resp);
        Assert.False(resp2);
        
        _smtpConfig.VerifyGet(x => x.Value, Times.Once);
        _resourceFiles.VerifyGet(x => x.Value, Times.Once);
        _pdfGeneratorService.Verify(x => x.ConvertHtmlStringToPdf(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _htmlGeneratorService.Verify(x => x.GenerateFromMarkdownString(It.IsAny<string>()), Times.Once);
        mailPackerMock.Verify(x => x.SetSmtpConfig(It.IsAny<SmtpServerConfiguration>()), Times.Exactly(2));
        //Because the previous mailPacker method is called it returns an exception
        mailPackerMock.Verify(x => x.SendAsync(), Times.Never);
    }
    
}