using MongoDB.Bson.Serialization;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Mapping;

public static class InventoryMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Inventory>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}