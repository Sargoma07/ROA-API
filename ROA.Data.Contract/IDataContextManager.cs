using ROA.Data.Contract.Locks;
using ROA.Data.Contract.Repositories;

namespace ROA.Data.Contract;

public interface IDataContextManager
{
    TRepository CreateRepository<TRepository>(string id = "default")
        where TRepository : class, IRepository;

    Task SaveAsync();

    IDataLock CreateLock(string lockId);
}