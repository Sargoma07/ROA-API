using MongoDB.Bson.Serialization;
using ROA.Model;

namespace ROA.Data.Mapping;

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