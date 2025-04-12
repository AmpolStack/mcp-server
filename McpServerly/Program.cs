using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace McpServerly;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.AddConsole(opts =>
        {
            opts.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services
            .AddLogging(opts => opts.AddConsole())
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        
        var host = builder.Build();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TempTool");
        TempTool.SetLogger(logger);
        
        TempTool.CallingTemp("call one"); 
        TempTool.CallingTemp("call two");
        
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
    
    [McpServerTool, Description("")]
    public static void CallingTemp(string message)
    {
        
        _counter++;
        var temp = $"Calling number {_counter}";
        _logger?.LogInformation(temp);
        _logger?.LogInformation(message);
    }

}