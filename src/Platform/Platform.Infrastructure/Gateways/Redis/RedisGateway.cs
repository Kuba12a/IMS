using Common.Application.Exceptions;
using Platform.Application.InfrastructureInterfaces;
using StackExchange.Redis;

namespace Platform.Infrastructure.Gateways.Redis;

public class RedisGateway : IInMemoryDatabaseGateway
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisGateway(RedisSettings redisSettings)
    {
        var options = ConfigurationOptions.Parse(redisSettings.ConnectionString);
        options.KeepAlive = 60;
        
        // options.Ssl = true;
        // options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        // options.SslHost = options.EndPoints[0].ToString();
        // options.CertificateValidation += (sender, certificate, chain, errors) => true;
        
        _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
    }

    public async Task IncrementExpiringCounterAsync(string key, TimeSpan expiresIn)
    {
        bool result;
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var transaction = db.CreateTransaction();
            transaction.StringIncrementAsync(key);
            transaction.KeyExpireAsync(key, expiresIn);
            result = await transaction.ExecuteAsync();
        }
        catch (Exception)
        {
            throw new RateLimitException("Failed to increment counter");
        }

        if (!result)
        {
            throw new RateLimitException("Failed to execute transaction for incrementing counter");
        }
    }

    public async Task RemoveAsync(string key)
    {
        bool result;
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var transaction = db.CreateTransaction();
            await transaction.KeyDeleteAsync(key);
            result = await transaction.ExecuteAsync();
        }
        catch (Exception)
        {
            throw new RateLimitException("Failed to remove key");
        }

        if (!result)
        {
            throw new RateLimitException("Failed to execute transaction for removing the key");
        }
    }

    public async Task<long> GetAsync(string key)
    {
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            return (long)await db.StringGetAsync(key);
        }
        catch (Exception)
        {
            throw new RateLimitException("Failed to get counter");
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        bool result;
        var exist = false;
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var transaction = db.CreateTransaction();
            var existTask = transaction.KeyExistsAsync(key);

            result = await transaction.ExecuteAsync();
            if(result)
            {
                exist = await existTask;
            }
        }
        catch (Exception)
        {
            throw new RateLimitException("Failed to check the key");
        }

        if (!result)
        {
            throw new RateLimitException("Failed to execute transaction for checking the key");
        }

        return !exist;
    }

    public async Task<TimeSpan> GetTimeToLiveAsync(string key)
    {
        bool result;
        TimeSpan? timespan = null;
        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var transaction = db.CreateTransaction();
            var timespanTask = transaction.KeyTimeToLiveAsync(key);
            result = await transaction.ExecuteAsync();
            if (result)
            {
                timespan = await timespanTask;
            }
        }
        catch (Exception)
        {
            throw new RateLimitException("Failed to check the key");
        }

        if (!result)
        {
            throw new RateLimitException("Failed to execute transaction for checking the key");
        }

        if (timespan == null)
        {
            throw new RateLimitException("Failed to obtain key's time to live");
        }

        return timespan.Value;
    }
}
