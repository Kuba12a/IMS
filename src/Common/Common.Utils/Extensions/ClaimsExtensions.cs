using System.Security.Claims;
using Common.Types.Enums;

namespace Common.Utils.Extensions;

public static class ClaimsExtensions
{
    public static bool TryGetGuid(this IEnumerable<Claim> claims, string claimType, out Guid guid)
    {
        guid = Guid.Empty;
        var claim = claims.FirstOrDefault(c => c.Type == claimType);
        return claim != null && Guid.TryParse(claim.Value, out guid);
    }
    
    public static bool TryGetLong(this IEnumerable<Claim> claims, string claimType, out long value)
    {
        value = default;
        var claim = claims.FirstOrDefault(c => c.Type == claimType);
        return claim != null && long.TryParse(claim.Value, out value);
    }
    
    public static bool TryGetTokenType(this IEnumerable<Claim> claims, string claimType, out SecurityTokenType value)
    {
        value = default;
        var claim = claims.FirstOrDefault(c => c.Type == claimType);
        return claim != null && Enum.TryParse(claim.Value, out value);
    }
}
