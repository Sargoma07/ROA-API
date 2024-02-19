using ROA.Model;

namespace ROA.Data.Contract.Repositories;

public interface IInventoryRepository: IRepository
{
    Task<IEnumerable<Inventory>> GetAllAsync();
    
    void AddOrUpdate(Inventory inventory);
    
    void Delete(Inventory inventory);
    
    Task<Inventory?> GetStorage(string playerId);
    
    Task<Inventory?> GetInventory(string playerId);
    
    Task<Inventory?> GetEquipment(string playerId);
}