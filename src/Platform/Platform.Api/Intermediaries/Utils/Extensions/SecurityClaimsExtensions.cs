using System.Security.Claims;
using Platform.Application.Constants;

namespace Platform.Api.Intermediaries.Utils.Extensions;

public static class SecurityClaimsExtensions
{
    public static Guid? GetUserId(this IEnumerable<Claim> claims)
    {
        var idClaim = claims?.FirstOrDefault(claim => claim.Type == AuthConstants.IdentityIdClaim);

        if (idClaim != null && Guid.TryParse(idClaim.Value, out var id))
        {
            return id;
        }

        return null;
    }

}
