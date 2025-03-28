using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Platform.Api.Intermediaries.Filters;
using Platform.Application.DependencyInjections;
using Platform.Application.Services.Auth;
using Platform.Infrastructure.DependencyInjections;
using Serilog;

namespace Platform.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, SecurityTokenSettings securityTokenSettings)
    {
        services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

        services
            .AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            })
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
        services.AddEndpointsApiExplorer();
        
        services.AddSwagger();

        // services.AddAuthentication(options =>
        //     {
        //         options.DefaultAuthenticateScheme = "Identities";
        //         options.DefaultChallengeScheme = "Identities";
        //     })
        //     .AddJwtBearer("Identities", options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = securityTokenSettings.GetTokenPublicKey(),
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ValidateLifetime = true,
        //             ClockSkew = TimeSpan.Zero
        //         };
        //     });
        //
        // services.AddAuthorizationBuilder()
        //             .AddPolicy("Identities", policy =>
        //     {
        //         policy.AddAuthenticationSchemes("Identities");
        //         policy.RequireAuthenticatedUser();
        //     });
        
        return services;
    }

    public static IApplicationBuilder UseWebApi(this IApplicationBuilder app, IWebHostEnvironment env,
        bool useCookie = true)
    {
        app.UseSerilogRequestLogging();

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
                applicationBuilder.AddServices(platformSettings.SecurityTokenSettings);
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
