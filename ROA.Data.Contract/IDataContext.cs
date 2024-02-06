using MongoDB.Driver;
using ROA.Model.Contract;

namespace ROA.Data.Contract;

public interface IDataContext : IDisposable
{
    void AddCommand(Func<Task> task);

    Task<int> SaveChanges();

    IDictionary<Type, IDictionary<string, IEntity>> ChangeTracker { get; }
    
    public IMongoDatabase Database { get; }

    public IClientSessionHandle Session { get; }
}