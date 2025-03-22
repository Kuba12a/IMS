using Common.Application.Exceptions;
using Common.Utils.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = Common.Application.Exceptions.ValidationException;

namespace Platform.Application.Common.Behavior;

public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IList<IValidator<TRequest>> _validators = new List<IValidator<TRequest>>();

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators,
        IServiceProvider serviceProvider)
    {
        foreach (var validator in validators)
        {
            _validators.Add(validator);
        }

        // handle validators for interfaces directly implemented by command like IListQuery
        foreach (var type in typeof(TRequest).GetInterfaces())
        {
            var interfaceValidators = serviceProvider.GetServices(typeof(IValidator<>).MakeGenericType(type));
            foreach (var validator in interfaceValidators)
            {
                _validators.Add((IValidator<TRequest>) validator!);
            }
        }
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .Select(err =>
                new ValidationError(err.ErrorMessage.RemoveAllApostrophes().RemoveDotIfIsAtTheEnd(), err.PropertyName))
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return next();
    }
}
