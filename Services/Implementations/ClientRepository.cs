using MongoDB.Driver;
using Services.Definitions;
using Services.Models;

namespace Services.Implementations;

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

    public async Task<Client> GetByCredentials(string phone, string email)
    {
        var query = await _clients.FindAsync(x => x.Email == email && x.Phone == phone);
        return await query.FirstAsync();
    }

    public async Task<bool> InsertAsync(Client client)
    {
        await _clients.InsertOneAsync(client);
        return true;
    }

    public async Task<bool> DeleteAsync(Client client)
    {
        var response = await _clients.DeleteOneAsync(x => x.Email == client.Email);
        return response.IsAcknowledged;
    }
    
}