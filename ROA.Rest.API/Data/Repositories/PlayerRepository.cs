using MongoDB.Driver.Linq;
using ROA.Infrastructure.Data;
using ROA.Infrastructure.Data.Mongo;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Repositories;

public class PlayerRepository : AbstractRepository<Player>, IPlayerRepository
{
    public PlayerRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Player?> GetByExternalId(string externalId)
    {
        return await GetQuery().SingleOrDefaultAsync(x =>
            x.ExternalId == externalId);
    }
}