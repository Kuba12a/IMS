using Common.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Platform.Api.Intermediaries.Filters;

    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = context.Exception switch
            {
                // Application Exceptions
                ValidationException e => new ObjectResult(new
                {
                    Type = "validation",
                    e.Errors
                })
                {
                    StatusCode = 400
                },
                LogicException e => new ObjectResult(new
                {
                    Type = "logic",
                    e.Message
                })
                {
                    StatusCode = 400
                },
                // Unknown
                _ => new ObjectResult(new
                {
                    Type = "internal"
                })
                {
                    StatusCode = 500
                }
            };

            if (result.StatusCode == 500)
            {
                if (!(context.Exception is OperationCanceledException))
                {
                    Log.Error(context.Exception, "Internal exception occured");
                }
            }

            context.ExceptionHandled = true;
            context.Result = result;
        }
    }
