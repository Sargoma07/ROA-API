using MongoDB.Bson.Serialization;
using ROA.Payment.API.Domain;

namespace ROA.Payment.API.Data.Mapping;

public static class AccountMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<Account>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}