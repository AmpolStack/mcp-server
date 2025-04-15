using Services.Models;

namespace Services.Implementations;

public interface IGenericRepository
{
    public Task<IEnumerable<Client>> GetAllAsync(CancellationToken ck = default);
    
}