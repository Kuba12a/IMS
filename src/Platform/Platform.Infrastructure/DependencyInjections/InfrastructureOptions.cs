using Common.Types.Settings;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using Platform.Infrastructure.Persistence;
using Platform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Domain.DomainServices;
using Platform.Infrastructure.DomainServices;

namespace Platform.Infrastructure.DependencyInjections;

public interface IInfrastructureOptions
{
    void AddModelWithPersistence(PostgresSettings postgresSettings);
}

internal class InfrastructureOptions : IInfrastructureOptions
{
    private readonly IServiceCollection _services;

    public InfrastructureOptions(IServiceCollection services)
    {
        _services = services;
    }

    public void AddModelWithPersistence(PostgresSettings postgresSettings)
    {
        _services.AddDbContext<WriteDbContext>(options =>
        {
            options
                .UseNpgsql(postgresSettings.ConnectionString, sqlServer => { })
                .UseSnakeCaseNamingConvention();
        });

        _services.AddDbContext<ReadDbContext>(options =>
        {
            options
                .UseNpgsql(postgresSettings.ConnectionString, sqlServer => {})
                .UseSnakeCaseNamingConvention();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }, ServiceLifetime.Transient);

        _services.AddTransient<IReadStorage, ReadStorage>();

        _services.AddScoped<IUnitOfWork, UnitOfWork>();

        AddRepositories();
        AddDomainServices();
    }

    private void AddRepositories()
    {
        _services.AddScoped<IIdentityRepository, IdentityRepository>();
    }
    
    private void AddDomainServices()
    {
        _services.AddScoped<IIdentityDomainService, IdentityDomainService>();
    }
}
