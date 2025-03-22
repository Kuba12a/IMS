using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Common.Behavior;

namespace Platform.Application.DependencyInjections;

public interface IApplicationOptions
{
    void AddCommandsAndQueries();
}

internal class ApplicationOptions : IApplicationOptions
{
    private readonly IServiceCollection _services;

    public ApplicationOptions(IServiceCollection services)
    {
        _services = services;
    }

    public void AddCommandsAndQueries()
    {
        _services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        _services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        
        _services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
