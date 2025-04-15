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
    
    public Task<string> GenerateFromMarkdownString(string markdownString, CancellationToken ck = default)
    {
        try
        {
            var html = Markdown.ToHtml(markdownString);
            return Task.FromResult(html);
        }
        catch(Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }
}