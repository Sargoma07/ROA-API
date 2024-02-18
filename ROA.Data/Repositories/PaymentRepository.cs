using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;

namespace ROA.Data.Repositories;

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