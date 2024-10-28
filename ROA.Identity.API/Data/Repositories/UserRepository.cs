using MongoDB.Driver.Linq;
using ROA.Identity.API.Domain;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;

namespace ROA.Identity.API.Data.Repositories;

public class UserRepository : AbstractRepository<User>, IUserRepository
{
    public UserRepository(IDataContext context) : base(context)
    {
    }

    public async Task<User?> GetByExternalId(string externalId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.ExternalId == externalId);
    }
}