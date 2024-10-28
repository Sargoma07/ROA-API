using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace ROA.Inventory.API.Data.Mapping;

public class DataContextMap
{
    private static bool _isMapsInitialized;
    
    public static void CreateMaps()
    {
        if (_isMapsInitialized)
        {
            return;
        }

        var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type)
                                                            || type.FullName.StartsWith("ROA.Inventory.API.Domain"));
        BsonSerializer.RegisterSerializer(objectSerializer);

        BsonSerializer.RegisterIdGenerator(typeof(string), StringObjectIdGenerator.Instance);

        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("EnumStringConvention", pack, t => true);

        CreateModelMap();

        _isMapsInitialized = true;
    }

    private static void CreateModelMap()
    {
        InventoryMap.CreateMap();
    }
}