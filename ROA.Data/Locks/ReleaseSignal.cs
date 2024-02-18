using MongoDB.Bson.Serialization.Attributes;

namespace ROA.Data.Locks;

public class ReleaseSignal
{
    [BsonId]
    public Guid AcquireId { get; set; }
}