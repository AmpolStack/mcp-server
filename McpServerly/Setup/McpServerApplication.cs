using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace McpServerly.Setup;

public static class McpServerApplication
{
    public static IHost Run(string[] args, Action<HostApplicationBuilder> configure)
    {
        //Create de Dependency injector for default application
        var builder = Host.CreateApplicationBuilder(args);
        
        var appsettingsPath = "appsettings.json";
        
        //because is necessary for the variable path of the appsettings.json
        if (args.Length > 0)
        {
            if (!string.IsNullOrEmpty(args[0]))
            {
                appsettingsPath = $"{args[0]}/" + appsettingsPath;
            }
        }
        
        //Include json file for configurations and enviroment
        builder.Configuration.AddJsonFile(appsettingsPath, optional: false, reloadOnChange: true);
        builder.Logging.AddConsole(opts =>
        {
            opts.LogToStandardErrorThreshold = LogLevel.Trace;
        });
        
        //Inject required services for run mcp server
        builder.Services
            .AddLogging(opts => opts.AddConsole())
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        
        //Inject the custom dependencies
        configure(builder);

        return builder.Build();
        
    }
}