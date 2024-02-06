using ROA.Data.Contract.Locks;

namespace ROA.Data.Locks;

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