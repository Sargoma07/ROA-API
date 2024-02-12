using ROA.Data.Contract;
using ROA.Data.Contract.Repositories;
using ROA.Model;
using MongoDB.Driver.Linq;

namespace ROA.Data.Repositories;

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