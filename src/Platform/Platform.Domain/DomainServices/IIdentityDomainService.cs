using Platform.Domain.Models.Identities;

namespace Platform.Domain.DomainServices;

public interface IIdentityDomainService
{
    Task<Identity> Login(string authCode, string codeVerifier, CancellationToken cancellationToken);
}
