namespace Common.Application.Exceptions;

public class ValidationError
{
    public string? PropertyName { get; }
    public string ErrorMessage { get; }

    public ValidationError(string errorMessage, string? propertyName = null)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}

public class ValidationException : ApplicationException
{
    public IEnumerable<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }
}
