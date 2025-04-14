using MongoDB.Driver;
using Services.Configurations;
using Services.Implementations;
using Services.Models;

namespace Services.Definitions;

public class ClientRepository : IGenericRepository
{
    private readonly IMongoCollection<Client> _clients;

    //TODO: Implements the singleton pattern in mongo client, and probably for all databases yet
    public ClientRepository(MongoConfiguration config)
    {
        var mongoClient = new MongoClient(config.GetConnectionString());
        var database = mongoClient.GetDatabase(config.Database);
        _clients = database.GetCollection<Client>("Clients");
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var query = await _clients.FindAsync(null);
        return await query.ToListAsync();
    }
}