using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public interface IInventoryRepository: IRepository
{
    Task<IEnumerable<Inventory>> GetAllAsync();
    
    void AddOrUpdate(Inventory inventory);
    
    void Delete(Inventory inventory);
    
    Task<Inventory?> GetStorage(string playerId);
    
    Task<Inventory?> GetInventory(string playerId);
    
    Task<Inventory?> GetEquipment(string playerId);
}