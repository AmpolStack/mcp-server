using System.ComponentModel;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
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
        var appsettingsPath = "C:/Users/htrsa/RiderProjects/McpServerly/McpServerly/appsettings.json";
        builder.Configuration.AddJsonFile(appsettingsPath, optional: false, reloadOnChange: true);
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
            .AddScoped<IPdfGeneratorService, PdfGeneratorService>()
            //Probably in the future this dependency has to change its lifetime to scoped
            .AddSingleton<IEmailService, EmailService>()
            .AddSingleton<IMailPacker, MailPack>();
        
        builder.Services
            .AddLogging(opts => opts.AddConsole())
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();
        
        
        var host = builder.Build();
        
        await host.RunAsync();
        
    }
}
