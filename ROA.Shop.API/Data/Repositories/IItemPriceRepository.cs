using ROA.Infrastructure.Data;
using ROA.Shop.API.Domain;

namespace ROA.Shop.API.Data.Repositories;

public interface IItemPriceRepository : IRepository
{
    Task<IEnumerable<ItemPrice>> GetPriceList(IEnumerable<string> uniqueNames);
}