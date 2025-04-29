using Services.Models;

namespace Services.Definitions;

public interface IGenericRepository
{
    public Task<IEnumerable<Client>> GetAllAsync(CancellationToken ck = default);
    public Task<Client> GetByCredentials(string phone, string email);
    public Task<bool> InsertAsync(Client client);
    public Task<bool> DeleteAsync(Client client);

}