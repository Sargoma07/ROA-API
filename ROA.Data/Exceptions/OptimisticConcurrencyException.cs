namespace ROA.Data.Exceptions;

public class OptimisticConcurrencyException: Exception
{
    public OptimisticConcurrencyException(string message)
        :base(message)
    {
    }
}