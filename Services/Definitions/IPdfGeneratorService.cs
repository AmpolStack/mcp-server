using Services.Custom;

namespace Services.Definitions;

public interface IPdfGeneratorService
{
    /// <summary>
    /// Converts HTML string in PDF file
    /// </summary>
    /// <param name="htmlContent">content in format HTML</param>
    /// <param name="outputPath">The output path without the filename</param>
    /// <returns>The new path file</returns>
    public Task<string> ConvertHtmlStringToPdf(string htmlContent, string outputPath);
}