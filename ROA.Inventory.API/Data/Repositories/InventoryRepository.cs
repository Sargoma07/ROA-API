using MongoDB.Driver.Linq;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Inventory.API.Domain.Types;

namespace ROA.Inventory.API.Data.Repositories;

public class InventoryRepository : AbstractRepository<Domain.Inventory>, IInventoryRepository
{
    public InventoryRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Domain.Inventory?> GetStorage(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.Storage);
    }

    public async Task<Domain.Inventory?> GetInventory(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.CharacterInventory);
    }

    public async Task<Domain.Inventory?> GetEquipment(string playerId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.PlayerId == playerId &&
            x.Type == InventoryType.Equipment);
    }
}