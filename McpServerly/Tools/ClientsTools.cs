using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Services.Definitions;
using Services.Models;

namespace McpServerly.Tools;

[McpServerToolType]
public class ClientsTools
{
    private static int _calls = 1;
    private readonly IGenericRepository? _repository;
    private readonly ILogger<ClientsTools> _logger;

    public ClientsTools(IGenericRepository repository, ILogger<ClientsTools> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [McpServerTool, Description("Returns list of current clients")]
    public async Task<List<Client>> GetClients()
    {
        _calls++;
        var resp = await _repository!.GetAllAsync();
        _logger.LogInformation("CLIENT TOOLS CALLED : [{calls}]", _calls);
        return resp.ToList();
    }
    
}