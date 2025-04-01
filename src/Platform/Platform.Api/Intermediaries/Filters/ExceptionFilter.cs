using System.Text;
using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Platform.Api.Intermediaries.Utils;
using Serilog;

namespace Platform.Api.Intermediaries.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var result = context.Exception switch
        {
            // Application Exceptions
            ValidationException e => new ObjectResultWrapper(new
            {
                Type = "validation",
                e.Errors
            })
            {
                StatusCode = 400,
                ExceptionLogType = ExceptionLogType.Validation
            },
            LogicException e => new ObjectResultWrapper(new
            {
                Type = "logic",
                e.Message,
            })
            {
                StatusCode = 400,
                ExceptionLogType = ExceptionLogType.Default
            },
            DomainException e => new ObjectResultWrapper(new
            {
                Type = "logic",
                e.Message,
            })
            {
                StatusCode = 400,
                ExceptionLogType = ExceptionLogType.Default
            },
            NotFoundException e => new ObjectResultWrapper(new
            {
                Type = "not_found",
                e.Message,
            })
            {
                StatusCode = 404,
                ExceptionLogType = ExceptionLogType.Default
            },
            AuthenticationException e => new ObjectResultWrapper(new
            {
                Type = "authentication",
                e.Message,
            })
            {
                StatusCode = 401,
                ExceptionLogType = ExceptionLogType.Default
            },
            // Unknown
            _ => new ObjectResultWrapper(new
            {
                Type = "internal"
            })
            {
                StatusCode = 500,
                ExceptionLogType = ExceptionLogType.Default
            }
        };

        LogException(result.ExceptionLogType, context.Exception);

        context.ExceptionHandled = true;
        context.Result = result;
    }

    private void LogException(ExceptionLogType exceptionLogType, Exception exception)
    {
        switch (exceptionLogType)
        {
            case ExceptionLogType.Validation:
                LogValidationApplicationException(exception);
                break;
            case ExceptionLogType.None:
                break;
            case ExceptionLogType.Default:
                Log.Error($"{exception.Message} in {exception.StackTrace}");
                break;
            default:
                Log.Error(exception.ToString());
                break;
        }
    }

    private void LogValidationApplicationException(Exception exception)
    {
        var validationApplicationException = (ValidationException)exception;
        var exceptionMessage = new StringBuilder(exception.Message);
        exceptionMessage.Append("\nThe following validation errors occured:");

        foreach (var validationError in validationApplicationException.Errors)
        {
            exceptionMessage.Append(
                $"\n => Property name: {validationError.PropertyName}; Error message: {validationError.ErrorMessage}");
        }

        Log.Error(exceptionMessage.ToString());
    }
}
