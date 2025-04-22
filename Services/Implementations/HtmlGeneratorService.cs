using Markdig;
using Microsoft.Extensions.Logging;
using Services.Definitions;

namespace Services.Implementations;

public class HtmlGeneratorService : IHtmlGeneratorService
{
    private readonly ILogger<HtmlGeneratorService> _logger;

    public HtmlGeneratorService(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<HtmlGeneratorService>();
    }
    
    public string GenerateFromMarkdownString(string markdownString)
    {
        if (string.IsNullOrEmpty(markdownString))
        {
            throw new ArgumentNullException(nameof(markdownString));
        }
        
        try
        {
            var html = Markdown.ToHtml(markdownString);
            return html;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to generate html from Markdown");
            throw new Exception(ex.Message);
        }
    }
}