using PuppeteerSharp;
using Services.Custom;
using Services.Definitions;

namespace Services.Implementations;

public class PdfGeneratorService : IPdfGeneratorService
{
    
    public async Task<FileResult> ConvertHtmlStringToPdf(string htmlContent, string outputPath)
    {
        var result = new FileResult();
        if (string.IsNullOrEmpty(htmlContent) || string.IsNullOrWhiteSpace(outputPath))
        {
            result.Success = false;
            return result;
        }
        
        try
        {
            var browserFetcher = new BrowserFetcher();
            var id = Guid.NewGuid().ToString();
            outputPath = Path.Combine(outputPath, id);
            outputPath += ".pdf";
            await browserFetcher.DownloadAsync(); 
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                Headless = true
            });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            await page.PdfAsync(outputPath);
            result.CompletePath = outputPath;
            result.Success = true;
            result.ExtensionPath = "pdf";
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            return result;
        }
    }
}