using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Services.Configurations;

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

        builder.Services
            .AddLogging(opts => opts.AddConsole())
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();


        var mongoConfig = new MongoConfiguration();
        builder.Configuration.GetSection("Databases:mongo").Bind(mongoConfig);
        
        
        var host = builder.Build();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TempTool");
        TempTool.SetLogger(logger);
        
        TempTool.GetLastCallingNumber("call one"); 
        TempTool.GetLastCallingNumber("call two");
        TempTool.GetNextCallingNumber("call three");
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