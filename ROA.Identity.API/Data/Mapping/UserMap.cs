using MongoDB.Bson.Serialization;
using ROA.Identity.API.Domain;

namespace ROA.Identity.API.Data.Mapping;

public static class UserMap
{
    public static void CreateMap()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
    }
}