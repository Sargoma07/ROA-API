using MongoDB.Driver;
using ROA.Infrastructure.Domain;

namespace ROA.Infrastructure.Data;

public interface IDataContext : IDisposable
{
    void AddCommand(Func<Task> task);

    Task<int> SaveChanges();

    IDictionary<Type, IDictionary<string, IEntity>> ChangeTracker { get; }
    
    public IMongoDatabase Database { get; }

    public IClientSessionHandle Session { get; }
}