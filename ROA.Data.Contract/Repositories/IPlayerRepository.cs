using ROA.Model;

namespace ROA.Data.Contract.Repositories;

public interface IPlayerRepository: IRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    
    void AddOrUpdate(Player inventory);
    
    void Delete(Player inventory);

    Task<Player?> GetByIdAsync(string id);
    
    Task<Player?> GetByExternalId(string externalId);
}