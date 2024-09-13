using MongoDB.Bson.Serialization.Attributes;

namespace ROA.Rest.API.Data.Locks;

public class LockLease : ILockLease
{
    [BsonId]
    public string Id { get; set; }
    public DateTime ExpiresIn { get; set; }
    public bool IsAcquired { get; set; }
    public Guid AcquireId { get; set; }
}