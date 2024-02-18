using MongoDB.Bson.Serialization.Attributes;
using ROA.Data.Contract.Locks;

namespace ROA.Data.Locks;

public class LockLease : ILockLease
{
    [BsonId]
    public string Id { get; set; }
    public DateTime ExpiresIn { get; set; }
    public bool IsAcquired { get; set; }
    public Guid AcquireId { get; set; }
}