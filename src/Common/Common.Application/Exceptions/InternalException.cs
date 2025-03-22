namespace Common.Application.Exceptions;

public class InternalException : ApplicationException
{
    public InternalException()
    {
    }

    public InternalException(string message) : base(message)
    {
    }
}
