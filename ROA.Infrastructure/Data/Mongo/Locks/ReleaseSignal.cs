using MongoDB.Bson.Serialization.Attributes;

namespace ROA.Infrastructure.Data.Mongo.Locks;

public class ReleaseSignal
{
    [BsonId]
    public Guid AcquireId { get; set; }
}