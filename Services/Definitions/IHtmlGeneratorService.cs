
namespace Services.Definitions;

public interface IHtmlGeneratorService
{
    public Task<string> GenerateFromMarkdownString(string markdownString, CancellationToken ck = default);
}