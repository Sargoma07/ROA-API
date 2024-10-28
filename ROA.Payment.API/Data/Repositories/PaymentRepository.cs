using MongoDB.Driver.Linq;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;

namespace ROA.Payment.API.Data.Repositories;

public class PaymentRepository(IDataContext context) : AbstractRepository<Domain.Payment>(context), IPaymentRepository
{
    public override void AddOrUpdate(Domain.Payment price)
    {
        if (string.IsNullOrEmpty(price.Id))
        {
            price.Created = DateTime.UtcNow;
        }
        
        price.Updated = DateTime.UtcNow;
        base.AddOrUpdate(price);
    }

    public async Task<Domain.Payment?> GetPaymentByAccount(string id, string accountId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.Id == id &&
            x.AccountId == accountId);
    }
}