namespace Common.Application.Exceptions;

public class RateLimitException : ApplicationException
{
    public RateLimitException()
    {
    }

    public RateLimitException(string message) : base(message)
    {
    }
}
