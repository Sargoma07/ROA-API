using MongoDB.Bson.Serialization;
using ROA.Model;

namespace ROA.Data.Mapping;

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