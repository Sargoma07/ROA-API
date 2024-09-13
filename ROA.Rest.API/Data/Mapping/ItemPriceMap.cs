using MongoDB.Bson.Serialization;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Mapping;

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