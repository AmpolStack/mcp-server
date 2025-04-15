using System.ComponentModel;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using MongoDB.Driver;
using Services.Configurations;
using Services.Definitions;
using Services.Implementations;
using Services.Models;

namespace McpServerly;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Logging.AddConsole(opts =>
        {
            opts.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        var mongoConfig = new MongoConfiguration();
        builder.Configuration.GetSection("Databases:mongo").Bind(mongoConfig);
        builder.Services
            .AddSingleton<IMongoClient>(prov =>
            {
                var mongoClient = new MongoClient(mongoConfig.GetConnectionString());
                return mongoClient;
            })
            .AddSingleton<IMongoDatabase>(prov =>
            {
                var client = prov.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoConfig.Database);
            })
            //Since this project is a test project, I don't really think it will include any other repository.
            .AddScoped<IGenericRepository, ClientRepository>()
            .AddScoped<IHtmlGeneratorService, HtmlGeneratorService>()
            .AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        
        builder.Services
            .AddLogging(opts => opts.AddConsole())
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        
        
        var host = builder.Build();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TempTool");

        var htmlGenerator = host.Services.GetRequiredService<IHtmlGeneratorService>();
        var markdown = "# hello world 2\r\n## subtitle\r\n*Hello World!*";
        var html = htmlGenerator.GenerateFromMarkdownString(markdown);

        var pdfGenerator = host.Services.GetRequiredService<IPdfGeneratorService>();
        var outputPath = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("Resources:filePath")!;
        var pdfResult = await pdfGenerator.ConvertHtmlStringToPdf(html, outputPath);
        
        TempTool.SetLogger(logger);
        
        await host.RunAsync();
        
    }
}

[McpServerToolType]
public static class TempTool
{
    private static int _counter = 0;
    private static ILogger? _logger;


    public static void SetLogger(ILogger? logger)
    {
        _logger = logger;
    }
    
    [McpServerTool, Description("returns the calling number current")]
    public static string GetLastCallingNumber(string message)
    {
        
        _counter++;
        var temp = $"Calling number {_counter}";
        _logger?.LogInformation(temp);
        _logger?.LogInformation(message);
        return temp;
    }

    [McpServerTool, Description("returns the next calling number")]
    public static string GetNextCallingNumber(string message)
    {
        var temp = $"The next call number is {_counter + 1}";
        _logger?.LogInformation(temp);
        _logger?.LogInformation(message);
        return temp;
    }
}