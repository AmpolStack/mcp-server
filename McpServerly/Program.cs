using McpServerly.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var host = McpServerApplication.Run(args, builder =>
        {
            var mongoConfig = new MongoConfiguration();
            builder.Configuration.GetSection("Databases:mongo").Bind(mongoConfig);
            
            builder.Services
                .AddSingleton<IMongoClient>((prov) =>
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
                .AddSingleton<IMailPacker, MailPack>()
                .Configure<SmtpServerConfiguration>(builder.Configuration.GetSection("SmtpServer"))
                .Configure<ResourceFiles>(builder.Configuration.GetSection("ResourceFiles"));
        });
        
        await host.RunAsync();
    }
}
