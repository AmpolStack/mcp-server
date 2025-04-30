using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Services.Configurations;
using Services.Definitions;

namespace McpServerly.Tools;

[McpServerToolType]
public class ReportsTool
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ReportsTool> _logger;
    private readonly IHtmlGeneratorService _htmlGeneratorService;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly SmtpServerConfiguration _smtpServerConfiguration;
    private readonly ResourceFiles _resourceFiles;
    private static readonly int Calls = 1;

    public ReportsTool(IEmailService emailService, ILogger<ReportsTool> logger, IHtmlGeneratorService htmlGeneratorService
    , IPdfGeneratorService pdfGeneratorService, IOptions<SmtpServerConfiguration> smtpServerConfiguration,
    IOptions<ResourceFiles> resourceFiles)
    {
        _emailService = emailService;
        _logger = logger;
        _htmlGeneratorService = htmlGeneratorService;
        _pdfGeneratorService = pdfGeneratorService;
        _smtpServerConfiguration = smtpServerConfiguration.Value;
        _resourceFiles = resourceFiles.Value;
    }

    [McpServerTool, Description("Receives a report as a markdown string, and the body mail message, and sends it by email")]
    public async Task<bool> SendReportWithMarkdown(string markdownString, string mailBody)
    {
        var htmlString = _htmlGeneratorService.GenerateFromMarkdownString(markdownString);
        var response = await SendPdfAndMail(htmlString, mailBody);

        return response;
    }
    
    [McpServerTool, Description("Receives a report as a html string, and the body mail message, and sends it by email")]
    public async Task<bool> SendReportWithHtml(string htmlString, string mailBody)
    {
        var response = await SendPdfAndMail(htmlString, mailBody);

        return response;
    }

    private async Task<bool> SendPdfAndMail(string htmlString, string mailBody)
    {
        var filePath = _resourceFiles.FilePath;
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        var fileResult = "";

        try
        {
            fileResult += await _pdfGeneratorService.ConvertHtmlStringToPdf(htmlString, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detected in pdf generation: {ex.Message}", ex.Message);
            return false;
        }

        var mailMessage = await _emailService
            .SetMessageSubject("Report #" + Calls + " IA GENERATED")
            .SetMessageBody(mailBody)
            .SetSenderEmail(_smtpServerConfiguration.Alias!, _smtpServerConfiguration.UserHost!)
            .AddReceiverAddress("temp User", "sacount571@gmail.com")
            .AddFile(fileResult, ".pdf")
            .BuildAsync();

        try
        {
            await mailMessage
                .SetSmtpConfig(_smtpServerConfiguration)
                .SendAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detected in send mail process: {ex.Message}" , ex.Message);
            return false;
        }
        
    }
    
}