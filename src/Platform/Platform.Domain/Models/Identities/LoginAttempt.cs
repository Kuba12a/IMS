using Common.Domain.Exceptions;
using Common.Utils;
using Common.Utils.Security;
using Platform.Domain.Common;
using Platform.Domain.Constants;
using Platform.Domain.Dtos;

namespace Platform.Domain.Models.Identities;

public class LoginAttempt : Entity
{
    public Guid Id { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public TwoFactorEmailAuthenticationChallenge? TwoFactorEmailAuthenticationChallenge { get; private set; }
    public AuthCodeChallenge? AuthCodeChallenge { get; private set; }
    public string CodeChallenge { get; private set; }

    internal static IdentityCreateLoginAttemptWithTwoFactorChallengeResult CreateWithTwoFactorEmailChallenge(string codeChallenge)
    {
        var emailCode = AlphanumericRandomStringGenerator.GenerateNumericCode();

        var sessionToken = AlphanumericRandomStringGenerator.GenerateAlphanumericToken();
        
        var loginAttempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            CreatedAt = Clock.Now,
            UpdatedAt = Clock.Now,
            TwoFactorEmailAuthenticationChallenge = new TwoFactorEmailAuthenticationChallenge(
                StringHasher.Hash(emailCode),
                StringHasher.Hash(sessionToken),
                Clock.Now,
                Clock.Now.Add(DomainConstants.TwoFactorChallengeDuration)),
            CodeChallenge = codeChallenge
        };
        
        return new IdentityCreateLoginAttemptWithTwoFactorChallengeResult(loginAttempt, sessionToken, emailCode);
    }
    
    internal static IdentityCreateLoginAttemptResult Create(string codeChallenge)
    {
        var authCode = AlphanumericRandomStringGenerator.GenerateAlphanumericToken();

        var loginAttempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            CreatedAt = Clock.Now,
            UpdatedAt = Clock.Now,
            CodeChallenge = codeChallenge,
            AuthCodeChallenge = new AuthCodeChallenge(StringHasher.Hash(authCode), Clock.Now,
                Clock.Now.Add(DomainConstants.AuthCodeChallengeDuration))
        };
        
        return new IdentityCreateLoginAttemptResult(loginAttempt, authCode);
    }

    internal string CheckTwoFactorChallenge(string twoFactorChallengeHash)
    {
        if (TwoFactorEmailAuthenticationChallenge?.CodeHash != twoFactorChallengeHash)
        {
            throw new DomainException(DomainExceptionMessages.TwoFactorCodeNotValid);
        }
        
        if (TwoFactorEmailAuthenticationChallenge.ValidTo < Clock.Now)
        {
            throw new DomainException(DomainExceptionMessages.TwoFactorCodeExpired);
        }

        var authCode = AlphanumericRandomStringGenerator.GenerateAlphanumericToken();

        AuthCodeChallenge = new AuthCodeChallenge(StringHasher.Hash(authCode), Clock.Now,
            Clock.Now.Add(DomainConstants.AuthCodeChallengeDuration));

        TwoFactorEmailAuthenticationChallenge = null;
        UpdatedAt = Clock.Now;

        return authCode;
    }
}
