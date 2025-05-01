using Platform.Domain.Models.Identities;
using Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Platform.Infrastructure.Persistence.Repositories;

internal class IdentityRepository : IIdentityRepository
{
    private readonly WriteDbContext _writeDbContext;
    private IQueryable<Identity> Identities => _writeDbContext.Identities
        .Include(i => i.LoginAttempts)
        .ThenInclude(la => la.AuthCodeChallenge)
        .Include(i => i.LoginAttempts)
        .ThenInclude(la => la.TwoFactorEmailAuthenticationChallenge)
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
    
    public async Task<Identity?> FirstOrDefaultByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultByAuthCodeHashAsync(string authCodeHash, CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.LoginAttempts.Any(la => la.AuthCodeChallenge != null
                                                                && la.AuthCodeChallenge.HashedCode == authCodeHash),
                cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultBySessionTokenHashAsync(string sessionTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.LoginAttempts
                    .Any(la =>  la.TwoFactorEmailAuthenticationChallenge != null 
                                && la.TwoFactorEmailAuthenticationChallenge.SessionTokenHash == sessionTokenHash),
                cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultByConfirmationTokenHashAsync(string confirmationTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.EmailConfirmationTokenHash == confirmationTokenHash, cancellationToken);
    }
    
    public async Task<Identity?> FirstOrDefaultByResetPasswordTokenHashAsync(string resetPasswordTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await Identities
            .FirstOrDefaultAsync(i => i.PasswordResetTokenHash == resetPasswordTokenHash, cancellationToken);
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
