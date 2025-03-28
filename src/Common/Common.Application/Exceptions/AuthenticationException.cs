namespace Common.Application.Exceptions;

public class AuthenticationException : ApplicationException
{
    public AuthenticationException() 
    {
    }

    public AuthenticationException(string message) : base(message)
    {
    }
}
