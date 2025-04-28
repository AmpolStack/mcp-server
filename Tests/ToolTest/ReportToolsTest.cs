using MailKit.Security;
using McpServerly.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Services.Configurations;
using Services.Definitions;
using Services.Implementations;
using Tests.Helpers;

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

        _smtpConfig
            .SetupGet(p => p.Value)
            .Returns(new SmtpServerConfiguration()
            {
                Alias = "test",
                Host = "testHost",
                Port = 20,
                Key = "testKey",
                SecureSocketOptions = SecureSocketOptions.StartTls,
                UserHost = "testUser",
            });
        
        _pdfGeneratorService
            .Setup(x => x.ConvertHtmlStringToPdf(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<Exception>();
        
        var service = new ReportsTool(_emailService.Object,
            _logger.Object,
            _htmlGeneratorService.Object,
            _pdfGeneratorService.Object,
            _smtpConfig.Object,
            _resourceFiles.Object);
        
        //Act
        var resp = await service.SendReportWithHtml("test subject", "# test body");
        var resp2 = await service.SendReportWithHtml("test subject", "# test body");
        
        //Assert
        Assert.False(resp);
        Assert.False(resp2);

    }

    [Fact]
    public async Task WhenTheMailSenderFails()
    {
        //Arrange
        var mailPackerMock = new Mock<IMailPacker>();
        mailPackerMock
            .Setup(m => m.SetSmtpConfig(It.IsAny<SmtpServerConfiguration>()))
            .Returns(mailPackerMock.Object);
        mailPackerMock
            .Setup(m => m.SendAsync())
            .ThrowsAsync(new Exception("SMTP failure"));

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
        var resp2 = await service.SendReportWithHtml("test subject", "# test body");
        
        //Assert
        Assert.False(resp);
        Assert.False(resp2);
    }
    
}