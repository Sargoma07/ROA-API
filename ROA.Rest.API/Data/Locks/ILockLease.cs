namespace ROA.Rest.API.Data.Locks;

public interface ILockLease
{
    public bool IsAcquired { get; }

    public Guid AcquireId { get; }
}