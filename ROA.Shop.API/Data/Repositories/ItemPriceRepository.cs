using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Infrastructure.Data.Mongo.Extensions;
using ROA.Shop.API.Domain;

namespace ROA.Shop.API.Data.Repositories;

public class ItemPriceRepository(IDataContext context) : AbstractRepository<ItemPrice>(context), IItemPriceRepository
{
    public async Task<IEnumerable<ItemPrice>> GetPriceList(IEnumerable<string> uniqueNames)
    {
        var prices =  await GetQuery()
            .Where(x => uniqueNames.Contains(x.UniqueName))
            .ToListAsync();
        
        return prices.LoadToContext(Context);
    }
}