namespace ROA.Infrastructure.Data.Locks;

public interface ILockLease
{
    public bool IsAcquired { get; }

    public Guid AcquireId { get; }
}