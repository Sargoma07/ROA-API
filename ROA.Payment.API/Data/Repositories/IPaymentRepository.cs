using ROA.Infrastructure.Data;

namespace ROA.Payment.API.Data.Repositories;

public interface IPaymentRepository: IRepository
{
    Task<IEnumerable<Domain.Payment>> GetAllAsync();
    
    void AddOrUpdate(Domain.Payment price);
    
    void Delete(Domain.Payment payment);

    Task<Domain.Payment?> GetPaymentByAccount(string id, string accountId);
    
}