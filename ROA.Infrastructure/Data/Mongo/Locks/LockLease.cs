using MongoDB.Bson.Serialization.Attributes;
using ROA.Infrastructure.Data.Locks;

namespace ROA.Infrastructure.Data.Mongo.Locks;

public class LockLease : ILockLease
{
    [BsonId]
    public string Id { get; set; }
    public DateTime ExpiresIn { get; set; }
    public bool IsAcquired { get; set; }
    public Guid AcquireId { get; set; }
}