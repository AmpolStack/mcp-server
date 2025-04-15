using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Services.Definitions;

namespace Services.Implementations;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly ILogger<PdfGeneratorService> _logger;

    public PdfGeneratorService(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<PdfGeneratorService>();
    }


    public async Task<bool> ConvertHtmlStringToPdf(string htmlContent, string outputPath)
    {
        try
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(); 
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                Headless = true
            });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            await page.PdfAsync(outputPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate pdf from HTML string");
            return false;
        }
    }
}