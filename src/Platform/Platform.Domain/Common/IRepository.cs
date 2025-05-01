namespace Platform.Domain.Common;

public interface IRepository<T> where T : Entity, IAggregate
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}
