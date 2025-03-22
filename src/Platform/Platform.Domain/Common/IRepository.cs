namespace Platform.Domain.Common;

public interface IRepository<T> where T : IAggregate
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}
