using PuppeteerSharp;
using Services.Custom;
using Services.Definitions;

namespace Services.Implementations;

public class PdfGeneratorService : IPdfGeneratorService
{
    
    public async Task<string> ConvertHtmlStringToPdf(string htmlContent, string outputPath)
    {
        if (string.IsNullOrEmpty(htmlContent) || string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(htmlContent) + " or " + nameof(outputPath) + " is null or empty");
        }
        
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
        return outputPath;
    }
}