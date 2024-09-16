using ROA.Infrastructure.Data;

namespace ROA.Inventory.API.Data.Repositories;

public interface IInventoryRepository: IRepository
{
    Task<IEnumerable<Domain.Inventory>> GetAllAsync();
    
    void AddOrUpdate(Domain.Inventory inventory);
    
    void Delete(Domain.Inventory inventory);
    
    Task<Domain.Inventory?> GetStorage(string playerId);
    
    Task<Domain.Inventory?> GetInventory(string playerId);
    
    Task<Domain.Inventory?> GetEquipment(string playerId);
}