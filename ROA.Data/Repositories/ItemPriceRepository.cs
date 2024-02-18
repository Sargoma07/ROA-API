using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;

namespace ROA.Data.Repositories;

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