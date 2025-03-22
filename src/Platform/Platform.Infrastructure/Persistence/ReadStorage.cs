using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Models.Identities;

namespace Platform.Infrastructure.Persistence;

internal class ReadStorage : IReadStorage
{
    public IQueryable<Identity> Identities => _readDbContext.Identities;

    private readonly ReadDbContext _readDbContext;

    public ReadStorage(ReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
}
