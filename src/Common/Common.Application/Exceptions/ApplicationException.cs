namespace Common.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    public override string? Message { get; }

    protected ApplicationException()
    {
    }

    protected ApplicationException(string? message)
    {
        Message = message;
    }
}
