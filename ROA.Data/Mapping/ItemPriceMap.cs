using MongoDB.Bson.Serialization;
using ROA.Model;

namespace ROA.Data.Mapping;

public static class ItemPriceMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<ItemPrice>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}