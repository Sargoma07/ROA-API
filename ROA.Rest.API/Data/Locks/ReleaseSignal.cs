using MongoDB.Bson.Serialization.Attributes;

namespace ROA.Rest.API.Data.Locks;

public class ReleaseSignal
{
    [BsonId]
    public Guid AcquireId { get; set; }
}