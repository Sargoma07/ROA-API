namespace ROA.Data.Contract.Locks;

public interface IDataLock
{
    Task<ILockLease> AcquireAsync(TimeSpan? lifetime = null, TimeSpan? timeout = null);

    Task ReleaseAsync(ILockLease lockAcquire);
}