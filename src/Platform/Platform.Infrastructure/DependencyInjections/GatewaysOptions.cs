using Microsoft.Extensions.DependencyInjection;
using Platform.Application.InfrastructureInterfaces;
using Platform.Infrastructure.Gateways.Redis;
using Platform.Infrastructure.Gateways.Smtp;

namespace Platform.Infrastructure.DependencyInjections;

public interface IGatewaysOptions
{
    void AddSmtpGateway(SmtpSettings smtpSettings);
    void AddRedisGateway(RedisSettings smtpSettings);
}

internal class GatewaysOptions : IGatewaysOptions
{
    private readonly IServiceCollection _services;

    public GatewaysOptions(IServiceCollection services)
    {
        _services = services;
    }

    public void AddSmtpGateway(SmtpSettings smtpSettings)
    {
        _services.AddSingleton<IEmailGateway>(new SmtpGateway(smtpSettings));
    }
    
    public void AddRedisGateway(RedisSettings redisSettings)
    {
        _services.AddSingleton<IInMemoryDatabaseGateway>(new RedisGateway(redisSettings));
    }
}
