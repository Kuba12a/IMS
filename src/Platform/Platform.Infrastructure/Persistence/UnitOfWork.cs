using Platform.Application.InfrastructureInterfaces;

namespace Platform.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _writeDbContext;

    public UnitOfWork(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}
