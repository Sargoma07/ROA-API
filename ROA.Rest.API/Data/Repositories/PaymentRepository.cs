using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public class PaymentRepository(IDataContext context) : AbstractRepository<Payment>(context), IPaymentRepository
{
    public override void AddOrUpdate(Payment price)
    {
        if (string.IsNullOrEmpty(price.Id))
        {
            price.Created = DateTime.UtcNow;
        }
        
        price.Updated = DateTime.UtcNow;
        base.AddOrUpdate(price);
    }
}