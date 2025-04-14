using MongoDB.Driver;
using Services.Configurations;
using Services.Implementations;
using Services.Models;

namespace Services.Definitions;

public class ClientRepository : IGenericRepository
{
    private readonly IMongoCollection<Client> _clients;
    
    public ClientRepository(IMongoDatabase database)
    {
        _clients = database.GetCollection<Client>("clients");
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var query = await _clients.FindAsync(_ => true);
        return await query.ToListAsync();
    }
}