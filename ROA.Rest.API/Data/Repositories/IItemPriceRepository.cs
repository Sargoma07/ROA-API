using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public interface IItemPriceRepository: IRepository
{
    Task<ItemPrice> GetPriceList();
}