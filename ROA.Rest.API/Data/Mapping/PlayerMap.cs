using MongoDB.Bson.Serialization;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Mapping;

public static class PlayerMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Player>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}