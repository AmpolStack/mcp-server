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

    public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken ck = default)
    {
        var query = await _clients.FindAsync(_ => true, cancellationToken: ck);
        return await query.ToListAsync(cancellationToken: ck);
    }
    
}