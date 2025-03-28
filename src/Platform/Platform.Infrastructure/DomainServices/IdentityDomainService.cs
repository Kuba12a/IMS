using Common.Domain.Exceptions;
using Common.Utils.Security;
using Platform.Domain.Constants;
using Platform.Domain.DomainServices;
using Platform.Domain.Models.Identities;
using Platform.Domain.Repositories;

namespace Platform.Infrastructure.DomainServices;

public class IdentityDomainService : IIdentityDomainService
{
    private readonly IIdentityRepository _identityRepository;

    public IdentityDomainService(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<Identity> Login(string authCode, string codeVerifier, CancellationToken cancellationToken)
    {
        var hashedAuthCode = StringHasher.Hash(authCode);
        var hashedCodeVerifier = StringHasher.Hash(codeVerifier);

        var identity = await _identityRepository
            .FirstOrDefaultByAuthCodeAsync(hashedAuthCode, cancellationToken);

        if (identity == null)
        {
            throw new DomainException(DomainExceptionMessages.LoginAttemptNotFound);
        }
        
        identity.Login(hashedAuthCode, hashedCodeVerifier);

        return identity;
    }
}
