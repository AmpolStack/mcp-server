using Markdig;
using Microsoft.Extensions.Logging;
using Services.Definitions;

namespace Services.Implementations;

public class HtmlGeneratorService : IHtmlGeneratorService
{
    
    public string GenerateFromMarkdownString(string markdownString)
    {
        if (string.IsNullOrEmpty(markdownString))
        {
            throw new ArgumentNullException(nameof(markdownString));
        }
        
        var html = Markdown.ToHtml(markdownString);
        return html;
        
    }
}