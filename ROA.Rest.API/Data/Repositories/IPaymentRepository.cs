using ROA.Infrastructure.Data;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public interface IPaymentRepository: IRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    
    void AddOrUpdate(Payment price);
    
    void Delete(Payment payment);

    Task<Payment?> GetByIdAsync(string id);
    
}