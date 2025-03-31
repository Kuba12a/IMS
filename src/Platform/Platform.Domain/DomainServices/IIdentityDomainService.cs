using Platform.Domain.Dtos;

namespace Platform.Domain.DomainServices;

public interface IIdentityDomainService
{
    Task<LoginResult> Login(
        string authCode,
        string codeVerifier,
        string ipAddress,
        CancellationToken cancellationToken);
}
