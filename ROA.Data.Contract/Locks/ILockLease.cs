namespace ROA.Data.Contract.Locks;

public interface ILockLease
{
    public bool IsAcquired { get; }

    public Guid AcquireId { get; }
}