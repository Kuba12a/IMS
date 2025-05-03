namespace Platform.Application.InfrastructureInterfaces;

public interface IInMemoryDatabaseGateway
{
    Task IncrementExpiringCounterAsync(string key, TimeSpan expiresIn);
    Task RemoveAsync(string key);
    Task<long> GetAsync(string key);
    Task<TimeSpan> GetTimeToLiveAsync(string key);
}
