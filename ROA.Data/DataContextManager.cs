using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ROA.Data.Contract;
using ROA.Data.Contract.Locks;
using ROA.Data.Contract.Repositories;
using ROA.Data.Locks;

namespace ROA.Data;

public class DataContextManager: IDataContextManager, IDisposable
{
    private readonly object _contextLock = new object();
    private Dictionary<string, DataContext> _contexts = new Dictionary<string, DataContext>();
    private readonly IOptions<ConnectionDatabaseSettings> _connectionSettings;
    private readonly IServiceProvider _container;

    public DataContextManager(IServiceProvider container, IOptions<ConnectionDatabaseSettings> connectionSettings)
    {
        _container = container;
        _connectionSettings = connectionSettings;
    }

    private DataContext GetDataContext(string id = "default")
    {
        var contextKey = id;

        lock (_contextLock)
        {

            if (!_contexts.ContainsKey(contextKey))
            {
                _contexts[contextKey] = new DataContext(_connectionSettings);
            }

            return _contexts[contextKey];
        }
    }

    public T CreateRepository<T>(string id = "default")
        where T : class, IRepository
    {
        var service = _container.GetService<T>();

        if (service == null)
        {
            throw new InvalidOperationException("Can't provide type from IOC");
        }
        
        return (T)ActivatorUtilities.CreateInstance(_container, service.GetType(), GetDataContext(id));
    }

    public async Task SaveAsync()
    {
        foreach (var context in _contexts.Values)
        {
            await context.SaveChanges();
        }
    }

    public void CloseConnections()
    {
        foreach (var context in _contexts.Values)
        {
            context.Dispose();
        }

        _contexts.Clear();
    }

    public void Dispose()
    {
        CloseConnections();
    }

    public IDataLock CreateLock(string lockId)
    {
        var context = GetDataContext();
        return new DataLock(context.Database, lockId);
    }
}