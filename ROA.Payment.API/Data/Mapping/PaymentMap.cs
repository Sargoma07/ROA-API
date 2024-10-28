using MongoDB.Bson.Serialization;

namespace ROA.Payment.API.Data.Mapping;

public static class PaymentMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Domain.Payment>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}