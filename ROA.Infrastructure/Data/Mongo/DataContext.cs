using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using ROA.Infrastructure.Domain;

namespace ROA.Infrastructure.Data.Mongo;

public class DataContext : IDataContext
{
    private readonly MongoClient _client;
    public IMongoDatabase Database { get; }

    public IClientSessionHandle Session
    {
        get
        {

            if (_session == null)
            {
                _session = _client.StartSession();
            }
            return _session;
        }
    }

    public IDictionary<Type, IDictionary<string, IEntity>> ChangeTracker { get; private set; }
        = new Dictionary<Type, IDictionary<string, IEntity>>();

    private readonly List<Func<Task>> _commands;
    private IClientSessionHandle _session;

    public DataContext(IOptions<ConnectionDatabaseSettings> connectionSettings)
    {
        var clientSettings = MongoClientSettings.FromConnectionString(connectionSettings.Value.ConnectionString);
        clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        _client = new MongoClient(clientSettings);
        Database = _client.GetDatabase(connectionSettings.Value.DatabaseName);
        _commands = new List<Func<Task>>();
    }
    
    public void AddCommand(Func<Task> task)
    {
        _commands.Add(task);
    }

    public async Task<int> SaveChanges()
    {
        var qtd = _commands.Count;

        foreach (var command in _commands)
        {
            await command();
        }

        _commands.Clear();
        
        return qtd;
    }

    public void Dispose()
    {
        if (_session != null)
        {
            _session.Dispose();
        }
    }
}