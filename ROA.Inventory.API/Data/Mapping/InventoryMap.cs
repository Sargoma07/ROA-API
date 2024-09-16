using MongoDB.Bson.Serialization;

namespace ROA.Inventory.API.Data.Mapping;

public static class InventoryMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Domain.Inventory>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}