using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

namespace Clients;

public static class ToolsClient
{
    public static async Task RunClient()
    {
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions()
        {
            Name = "McpServerly",
            Command = "dotnet",
            Arguments = [
                "run",
                "--project",
                "C:/Users/htrsa/RiderProjects/McpServerly/McpServerly/McpServerly.csproj",
                "--",
                "C:/Users/htrsa/RiderProjects/McpServerly/McpServerly/"
            ]
        }); 
        
        var client = await McpClientFactory.CreateAsync(clientTransport);
        var tools = await client.ListToolsAsync();
        foreach (var tool in tools)
        {
            Console.WriteLine(tool.Name + " : " + tool.ProtocolTool + " : " + tool.Description);
        }

        var result = await client.CallToolAsync(
            "GetClients",
            null,
            null,
            null,
            CancellationToken.None
        );
        
        Console.WriteLine(result.Content.First(c => c.Type == "text").Text);
    }
}