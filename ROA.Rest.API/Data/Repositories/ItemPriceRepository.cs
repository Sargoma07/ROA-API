using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public class ItemPriceRepository(IDataContext context) : AbstractRepository<ItemPrice>(context), IItemPriceRepository
{
    public async Task<ItemPrice> GetPriceList()
    {
        const string validItemPriceId = "632468860d45ec833dcbb83e"; 
        var itemPrice = await GetByIdAsync(validItemPriceId);
        
        if (itemPrice is null)
        {
            throw new InvalidOperationException($"No found item price {validItemPriceId}");
        }
        
        return itemPrice;
    }
}