using ROA.Infrastructure.Data;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.Data.Repositories;

public interface IAccountRepository: IRepository
{
    void AddOrUpdate(Account account);
    
    void Add(Account account);
    
    void Delete(Account account);

    Task<Account?> GetByIdAsync(string id);
}