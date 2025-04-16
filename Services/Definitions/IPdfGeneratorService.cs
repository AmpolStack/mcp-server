using Services.Custom;

namespace Services.Definitions;

public interface IPdfGeneratorService
{
    public Task<FileResult> ConvertHtmlStringToPdf(string htmlContent, string outputPath);
}