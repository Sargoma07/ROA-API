using MongoDB.Bson.Serialization;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.Data.Mapping;

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