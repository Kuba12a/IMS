using Common.Domain.Exceptions;
using Common.Utils.Security;
using Platform.Application.Services.Auth;
using Platform.Domain.Constants;
using Platform.Domain.DomainServices;
using Platform.Domain.Dtos;
using Platform.Domain.Repositories;

namespace Platform.Infrastructure.DomainServices;

public class IdentityDomainService : IIdentityDomainService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly ISecurityTokenService _securityTokenService;

    public IdentityDomainService(IIdentityRepository identityRepository, ISecurityTokenService securityTokenService)
    {
        _identityRepository = identityRepository;
        _securityTokenService = securityTokenService;
    }

    public async Task<LoginResult> Login(string authCode, string codeVerifier, string ipAddress,
        CancellationToken cancellationToken)
    {
        var hashedAuthCode = StringHasher.Hash(authCode);
        var hashedCodeVerifier = StringHasher.Hash(codeVerifier);

        var identity = await _identityRepository
            .FirstOrDefaultByAuthCodeHashAsync(hashedAuthCode, cancellationToken);

        if (identity == null)
        {
            throw new DomainException(DomainExceptionMessages.LoginAttemptNotFound);
        }
        
        identity.RemoveLoginAttempt(hashedAuthCode, hashedCodeVerifier);

        var idToken = _securityTokenService.CreateIdToken(identity.Id);
        var accessToken = _securityTokenService.CreateAccessToken(identity.Id);
        var refreshToken = _securityTokenService.CreateRefreshToken(identity.Id);
        
        identity.AddSession(StringHasher.Hash(refreshToken.Value), ipAddress);

        return new LoginResult(identity, idToken, accessToken, refreshToken);
    }
}
