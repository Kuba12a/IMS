using System.Reflection;
using Platform.Infrastructure.Persistence;
using  Platform.Migrator;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var settings = new IdentitiesMigratorSettings();
        hostContext.Configuration.Bind(nameof(IdentitiesMigratorSettings), settings);
        settings.Validate();
        services.AddSingleton(settings);

        services.AddDbContext<WriteDbContext>(options =>
        {
            options.UseNpgsql(
                    settings.PostgresSettings.GetConnectionString(),
                    sqlServer =>
                    {
                        sqlServer.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    })
                .UseSnakeCaseNamingConvention();
        });
    })
    .Build();

using var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<WriteDbContext>();

applicationDbContext.Database.Migrate();
