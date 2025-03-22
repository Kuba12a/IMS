using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
    }

}
