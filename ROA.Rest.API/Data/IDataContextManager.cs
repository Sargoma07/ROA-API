using ROA.Rest.API.Data.Locks;
using ROA.Rest.API.Data.Repositories;

namespace ROA.Rest.API.Data;

public interface IDataContextManager
{
    TRepository CreateRepository<TRepository>(string id = "default")
        where TRepository : class, IRepository;

    Task SaveAsync();

    IDataLock CreateLock(string lockId);
}