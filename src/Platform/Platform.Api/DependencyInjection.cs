using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Platform.Api.Intermediaries.Filters;
using Platform.Api.Intermediaries.Middlewares;
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
                options.Filters.Add<AuthenticationContextAuthorizationFilter>();
            })
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Identities";
                options.DefaultChallengeScheme = "Identities";
            })
            .AddJwtBearer("Identities", options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
                        {
                            context.Token = token;
                        }
            
                        return Task.CompletedTask;
                    }
                };
    
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityTokenSettings.GetTokenPublicKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Identities", policy =>
            {
                policy.AddAuthenticationSchemes("Identities");
                policy.RequireAuthenticatedUser();
            });
        
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
            .SetIsOriginAllowed(_ => true)
            // .WithOrigins("https://app.example.localhost:3001", "https://api.example.localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
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
                applicationBuilder.AddServices(platformSettings.SecurityTokenSettings, platformSettings.PasswordSettings);
            })
            .AddInfrastructure(infrastructureBuilder =>
            {
                infrastructureBuilder.AddModelWithPersistence(platformSettings.PostgresSettings);
                infrastructureBuilder.AddGateways(gatewaysBuilder =>
                {
                    gatewaysBuilder.AddSmtpGateway(platformSettings.SmtpSettings);
                });
            });
        
        services.AddHttpContextAccessor();

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
