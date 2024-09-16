namespace ROA.Infrastructure.Data.Mongo.Exceptions;

public class OptimisticConcurrencyException: Exception
{
    public OptimisticConcurrencyException(string message)
        :base(message)
    {
    }
}