namespace ROA.Rest.API.Data.Locks;

internal class AcquireResult : ILockLease
{
    public AcquireResult(Guid acquireId)
    {
        IsAcquired = true;
        AcquireId = acquireId;
    }

    public AcquireResult()
    {
        IsAcquired = false;
    }

    public bool IsAcquired { get; private set; }

    public Guid AcquireId { get; private set; }
}