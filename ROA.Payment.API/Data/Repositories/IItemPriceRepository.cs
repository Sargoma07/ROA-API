using ROA.Infrastructure.Data;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.Data.Repositories;

public interface IItemPriceRepository: IRepository
{
    Task<ItemPrice> GetPriceList();
}