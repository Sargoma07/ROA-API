namespace ROA.Infrastructure.Data.Mongo.Exceptions;

public class LockLeaseException: Exception
{
    public LockLeaseException(string message)
        :base(message)
    {
    }
}