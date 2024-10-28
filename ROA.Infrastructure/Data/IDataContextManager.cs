using ROA.Infrastructure.Data.Locks;

namespace ROA.Infrastructure.Data;

public interface IDataContextManager
{
    TRepository CreateRepository<TRepository>(string id = "default")
        where TRepository : class, IRepository;

    Task SaveAsync();

    IDataLock CreateLock(string lockId);
}