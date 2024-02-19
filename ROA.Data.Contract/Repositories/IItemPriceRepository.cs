using ROA.Model;

namespace ROA.Data.Contract.Repositories;

public interface IItemPriceRepository: IRepository
{
    Task<ItemPrice> GetPriceList();
}