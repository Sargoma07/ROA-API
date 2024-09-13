using MongoDB.Driver.Linq;
using ROA.Rest.API.Domain;
using ROA.Rest.API.Domain.Types;

namespace ROA.Rest.API.Data.Repositories;

public class InventoryRepository : AbstractRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetStorage(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.Storage);
    }

    public async Task<Inventory?> GetInventory(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.CharacterInventory);
    }

    public async Task<Inventory?> GetEquipment(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.Equipment);
    }
}