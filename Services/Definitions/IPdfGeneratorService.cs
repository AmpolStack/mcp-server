namespace Services.Definitions;

public interface IPdfGeneratorService
{
    public Task<bool> ConvertHtmlStringToPdf(string htmlContent, string outputPath);
}