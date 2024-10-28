using Refit;

namespace ROA.Payment.API.ServiceAPI.ShopService;

public interface IShopServiceApi
{
    [Get("/prices/{currency}")]
    Task<IList<ItemPriceModel>> GetPrices(string currency,
        [Query(CollectionFormat.Multi)] IEnumerable<string> uniqueNames);
}