using Microsoft.Extensions.DependencyInjection;

namespace Platform.Application.DependencyInjections;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        Action<IApplicationOptions> builder)
    {
        var options = new ApplicationOptions(services);
        builder(options);

        return services;
    }
}
