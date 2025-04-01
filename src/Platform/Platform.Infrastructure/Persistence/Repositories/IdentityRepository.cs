using Platform.Domain.Models.Identities;
using Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Platform.Infrastructure.Persistence.Repositories;

internal class IdentityRepository : IIdentityRepository
{
    private readonly WriteDbContext _writeDbContext;
    private IQueryable<Identity> Identities => _writeDbContext.Identities
        .Include(i => i.LoginAttempts)
        .Include(i => i.Sessions);

    public IdentityRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Identity?> FirstOrDefaultByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.Email == email, cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultByAuthCodeAsync(string authCode, CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.LoginAttempts.Any(la => la.AuthCode == authCode),
                cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultByConfirmationTokenHashAsync(string confirmationTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.EmailConfirmationTokenHash == confirmationTokenHash, cancellationToken);
    }

    public async Task AddAsync(Identity entity, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Identities.AddAsync(entity, cancellationToken);
    }

    public void Remove(Identity entity)
    {
        _writeDbContext.Remove(entity);
    }
}
