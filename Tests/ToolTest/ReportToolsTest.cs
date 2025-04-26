using MailKit.Security;
using McpServerly.Tools;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Services.Configurations;
using Services.Definitions;
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
    private readonly ReportsTool _service;
    public ReportToolsTest()
    {
        _emailService = new Mock<IEmailService>();
        _logger = new Mock<ILogger<ReportsTool>>();
        _htmlGeneratorService = new Mock<IHtmlGeneratorService>();
        _pdfGeneratorService = new Mock<IPdfGeneratorService>();
        _smtpConfig = new Mock<IOptions<SmtpServerConfiguration>>();
        _resourceFiles = new Mock<IOptions<ResourceFiles>>();
        
        _service = new ReportsTool(_emailService.Object,
            _logger.Object,
            _htmlGeneratorService.Object,
            _pdfGeneratorService.Object,
            _smtpConfig.Object,
            _resourceFiles.Object);
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
        var resp = await service.SendReportWithHtml("lamane", "vamooooooo");
        
        //Assert
        Assert.False(resp);
        
    }
}