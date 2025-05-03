using Common.Application.Exceptions;
using Platform.Application.InfrastructureInterfaces;

namespace Platform.Application.Services.RateLimit;

public interface IRateLimitService
{
    Task TryIncrementAttemptCounterAsync(string keyPrefix, string identifier, string message, int attemptLimit,
        TimeSpan keyExpiresIn);
    Task ResetAttemptCounterAsync(string keyPrefix, string identifier);
}

public class RateLimitService : IRateLimitService
{
    private readonly IInMemoryDatabaseGateway _inMemoryDatabaseGateway;
    private static string Key(string keyPrefix, string identifier) => $"{keyPrefix}:{identifier}".ToLower();

    public RateLimitService(IInMemoryDatabaseGateway inMemoryDatabaseGateway)
    {
        _inMemoryDatabaseGateway = inMemoryDatabaseGateway;
    }

    public async Task TryIncrementAttemptCounterAsync(string keyPrefix, string identifier, string message,
        int attemptLimit, TimeSpan keyExpiresIn)
    {
        if (await IsAttemptCounterAtLimitAsync(keyPrefix, identifier, attemptLimit))
        {
            var waitDuration = await GetWaitDurationAsync(keyPrefix, identifier);
            var seconds = Math.Ceiling(waitDuration.TotalSeconds);

            throw new RateLimitException($"{message}. Please try again in {seconds} seconds");
        }

        var counterKey = Key(keyPrefix, identifier);

        await _inMemoryDatabaseGateway.IncrementExpiringCounterAsync(counterKey, keyExpiresIn);
    }

    public async Task ResetAttemptCounterAsync(string keyPrefix, string identifier)
    {
        await _inMemoryDatabaseGateway.RemoveAsync(Key(keyPrefix, identifier));
    }

    private async Task<TimeSpan> GetWaitDurationAsync(string keyPrefix, string identifier)
    {
        return await _inMemoryDatabaseGateway.GetTimeToLiveAsync(Key(keyPrefix, identifier));
    }

    private async Task<bool> IsAttemptCounterAtLimitAsync(string keyPrefix, string identifier, int attemptLimit)
    {
        return await _inMemoryDatabaseGateway.GetAsync(Key(keyPrefix, identifier)) >= attemptLimit;
    }
}
