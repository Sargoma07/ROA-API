using ROA.Model;

namespace ROA.Data.Contract.Repositories;

public interface IPaymentRepository: IRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    
    void AddOrUpdate(Payment price);
    
    void Delete(Payment payment);

    Task<Payment?> GetByIdAsync(string id);
    
}