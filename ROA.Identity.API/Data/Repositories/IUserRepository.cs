using ROA.Identity.API.Domain;
using ROA.Infrastructure.Data;

namespace ROA.Identity.API.Data.Repositories;

public interface IUserRepository: IRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    
    void AddOrUpdate(User user);
    
    void Delete(User user);

    Task<User?> GetByIdAsync(string id);
    
    Task<User?> GetByExternalId(string externalId);
}