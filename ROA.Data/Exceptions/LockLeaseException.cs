namespace ROA.Data.Exceptions;

public class LockLeaseException: Exception
{
    public LockLeaseException(string message)
        :base(message)
    {
    }
}