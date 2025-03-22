using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Platform.Application.DependencyInjections;
using Platform.Infrastructure.DependencyInjections;

namespace Platform.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
        services.AddEndpointsApiExplorer();
        
        services.AddSwagger();

        return services;
    }

    public static IApplicationBuilder UseWebApi(this IApplicationBuilder app, IWebHostEnvironment env,
        bool useCookie = true)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
    
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Platform API v1");
                c.RoutePrefix = string.Empty;
            });
        }
        
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials());

        app.UseRouting();

        if (useCookie)
        {
            app.UseCookiePolicy();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
    
    public static IServiceCollection AddPlatform(this IServiceCollection services,
        PlatformSettings platformSettings)
    {
        services
            .AddApplication(applicationBuilder =>
            {
                applicationBuilder.AddCommandsAndQueries();
            })
            .AddInfrastructure(infrastructureBuilder =>
            {
                infrastructureBuilder.AddModelWithPersistence(platformSettings.PostgresSettings);
            });

        return services;
    }
    
    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "T",
                Description = "D"
            });
        });
    }
}
