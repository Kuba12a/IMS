namespace Common.Application.Exceptions;

public class LogicException : ApplicationException
{
    public LogicException()
    {
    }

    public LogicException(string message) : base(message)
    {
    }
}
