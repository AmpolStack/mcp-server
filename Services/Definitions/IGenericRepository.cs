using Services.Models;

namespace Services.Definitions;

public interface IGenericRepository
{
    public Task<IEnumerable<Client>> GetAllAsync(CancellationToken ck = default);
    
}