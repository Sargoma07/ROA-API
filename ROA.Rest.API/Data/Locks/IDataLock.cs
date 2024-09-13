namespace ROA.Rest.API.Data.Locks;

public interface IDataLock
{
    Task<ILockLease> AcquireAsync(TimeSpan? lifetime = null, TimeSpan? timeout = null);

    Task ReleaseAsync(ILockLease lockAcquire);
}