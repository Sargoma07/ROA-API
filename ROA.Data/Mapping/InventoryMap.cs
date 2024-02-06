using MongoDB.Bson.Serialization;
using ROA.Model;

namespace ROA.Data.Mapping;

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