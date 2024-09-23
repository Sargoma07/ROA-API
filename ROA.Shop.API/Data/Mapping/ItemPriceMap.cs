using MongoDB.Bson.Serialization;
using ROA.Shop.API.Domain;

namespace ROA.Shop.API.Data.Mapping;

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