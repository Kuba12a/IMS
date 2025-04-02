using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Common.Behavior;
using Platform.Application.Services.Auth;
using Platform.Application.Services.UrlBuilder;

namespace Platform.Application.DependencyInjections;

public interface IApplicationOptions
{
    void AddCommandsAndQueries();
    void AddServices(SecurityTokenSettings securityTokenSettings, UrlBuilderSettings urlBuilderSettings);
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
    
    public void AddServices(SecurityTokenSettings securityTokenSettings, UrlBuilderSettings urlBuilderSettings)
    {
        _services.AddSingleton(securityTokenSettings);
        
        _services.AddSingleton<ISecurityTokenService, SecurityTokenService>();

        _services.AddSingleton<IUrlBuilderService>(_ => new UrlBuilderService(urlBuilderSettings));
    }
}
