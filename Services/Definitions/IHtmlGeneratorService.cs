
namespace Services.Definitions;

public interface IHtmlGeneratorService
{
    public string GenerateFromMarkdownString(string markdownString);
}