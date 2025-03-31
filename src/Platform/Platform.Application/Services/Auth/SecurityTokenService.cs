using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common.Utils;
using Microsoft.IdentityModel.Tokens;
using Platform.Application.Constants;
using Serilog;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Platform.Application.Services.Auth;

public interface ISecurityTokenService
{
    string CreateAccessToken(Guid identityId);
    string CreateIdToken(Guid identityId);
    string CreateRefreshToken(Guid identityId);
}

public class SecurityTokenService : ISecurityTokenService
{
    private readonly SecurityTokenSettings _securityTokenSettings;
    
    private readonly RsaSecurityKey _privateKey;
    private readonly RsaSecurityKey _publicKey;
    
    public SecurityTokenService(SecurityTokenSettings securityTokenSettings)
    {
        _securityTokenSettings = securityTokenSettings;

        _privateKey = _securityTokenSettings.GetTokenPrivateKey();
        _publicKey = _securityTokenSettings.GetTokenPublicKey();
    }

    public string CreateAccessToken(Guid identityId)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim(AuthConstants.TokenTypeClaim, SecurityTokenType.AccessToken.ToString()),
            new Claim(AuthConstants.IdentityIdClaim, identityId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = (Clock.Now + TimeSpan.FromSeconds(_securityTokenSettings.AccessTokenDurationInSeconds)).UtcDateTime,
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    
    public string CreateIdToken(Guid identityId)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim(AuthConstants.TokenTypeClaim, SecurityTokenType.IdToken.ToString()),
            new Claim(AuthConstants.IdentityIdClaim, identityId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = (Clock.Now + TimeSpan.FromSeconds(_securityTokenSettings.IdTokenDurationInSeconds)).UtcDateTime,
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(Guid identityId)
    {
        var subject = new ClaimsIdentity(new[]
        {
            new Claim(AuthConstants.TokenTypeClaim, SecurityTokenType.RefreshToken.ToString()),
            new Claim(AuthConstants.IdentityIdClaim, identityId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Expires = (Clock.Now + TimeSpan.FromSeconds(_securityTokenSettings.RefreshTokenDurationInSeconds)).UtcDateTime,
            SigningCredentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    
    private ClaimsPrincipal? ReadAndValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            return tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = _publicKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error occured during token validation");
            return null;
        }
    }
}
