using MongoDB.Bson.Serialization;
using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Mapping;

public static class PaymentMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Payment>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}