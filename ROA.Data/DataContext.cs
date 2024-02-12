using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ROA.Data.Contract;
using ROA.Data.Mapping;
using ROA.Model.Contract;

namespace ROA.Data;

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
    private static bool _isMapsInitialized;

    public DataContext(IOptions<ConnectionDatabaseSettings> connectionSettings)
    {
        _client = new MongoClient(connectionSettings.Value.ConnectionString);
        Database = _client.GetDatabase(connectionSettings.Value.DatabaseName);
        _commands = new List<Func<Task>>();
    }
    
    public static void CreateMaps()
    {
        if (_isMapsInitialized)
        {
            return;
        }

        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type)
                                                            || type.FullName.StartsWith("ROA.Model"));
        BsonSerializer.RegisterSerializer(objectSerializer);

        BsonSerializer.RegisterIdGenerator(typeof(string), StringObjectIdGenerator.Instance);

        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("EnumStringConvention", pack, t => true);

        CreateModelMap();
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
    
    private static void CreateModelMap()
    {
        InventoryMap.CreateMap();
        PlayerMap.CreateMap();
    }
}