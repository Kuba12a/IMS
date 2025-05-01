using Common.Domain.Exceptions;
using Common.Utils;
using Common.Utils.Security;
using Platform.Domain.Common;
using Platform.Domain.Constants;
using Platform.Domain.Dtos;

namespace Platform.Domain.Models.Identities;

public class Identity : Entity, IAggregate
{
    public Guid Id { get; private init; }
    public string Email { get; private init; }
    public string? PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public string? PasswordResetTokenHash { get; private set; }
    public DateTimeOffset? PasswordResetTokenValidTo { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public string? EmailConfirmationTokenHash { get; private set; }
    public DateTimeOffset? EmailConfirmationTokenValidTo { get; private set; }
    public List<LoginAttempt> LoginAttempts { get; private init; }
    public List<Session> Sessions { get; private init; }
    public bool RequireMfa { get; set; }
    public static IdentityCreateResult Create(string email, string password)
    {
        var emailConfirmationToken = AlphanumericRandomStringGenerator
            .GenerateAlphanumericToken(DomainConstants.EmailConfirmationTokenLength);

        return new IdentityCreateResult(new Identity
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower(),
            PasswordHash = PasswordHasher.Hash(password),
            CreatedAt = Clock.Now,
            UpdatedAt = Clock.Now,
            EmailConfirmed = false,
            EmailConfirmationTokenHash = StringHasher.Hash(emailConfirmationToken),
            EmailConfirmationTokenValidTo = Clock.Now + DomainConstants.EmailConfirmationTokenDuration,
            LoginAttempts = [],
            Sessions = [],
            RequireMfa = false
        }, emailConfirmationToken);
    }
    
    public IdentityInitiateLoginResult InitiateLogin(string password, string codeChallenge)
    {
        if (PasswordHash == default || !PasswordHasher.Verify(password, PasswordHash))
        {
            throw new DomainException("Invalid Credentials");
        }

        if (!EmailConfirmed)
        {
            throw new DomainException("Email not confirmed");
        }

        if (RequireMfa)
        {
            var loginAttemptCreateResult = LoginAttempt.CreateWithTwoFactorEmailChallenge(codeChallenge);
            LoginAttempts.Add(loginAttemptCreateResult.LoginAttempt);
            
            UpdatedAt = Clock.Now;

            return new IdentityInitiateLoginWithEmailCodeResult(loginAttemptCreateResult.SessionToken,
                loginAttemptCreateResult.EmailCode,
                loginAttemptCreateResult.LoginAttempt.TwoFactorEmailAuthenticationChallenge!.ValidTo);
        }

        else
        {
            var loginAttemptCreateResult = LoginAttempt.Create(codeChallenge);
            LoginAttempts.Add(loginAttemptCreateResult.LoginAttempt);
            
            UpdatedAt = Clock.Now;

            return new IdentityInitiateLoginWithoutTwoFactorAuthResult(loginAttemptCreateResult.AuthCode);
        }
    }

    public string CheckTwoFactorEmailChallenge(string sessionTokenHash,
        string twoFactorChallengeHash)
    {
        var loginAttempt = LoginAttempts.FirstOrDefault(la => la.TwoFactorEmailAuthenticationChallenge?
            .SessionTokenHash == sessionTokenHash);

        if (loginAttempt == default)
        {
            throw new DomainException(DomainExceptionMessages.InvalidToken);
        }

        return loginAttempt
            .CheckTwoFactorChallenge(twoFactorChallengeHash);
    }

    public void RemoveLoginAttempt(string hashedAuthCode, string hashedCodeVerifier)
    {
        var loginAttempt = LoginAttempts
            .FirstOrDefault(la =>
                la.AuthCodeChallenge?.HashedCode == hashedAuthCode &&
                la.CodeChallenge == hashedCodeVerifier);

        if (loginAttempt == default)
        {
            throw new DomainException(DomainExceptionMessages.LoginAttemptNotFound);
        }

        LoginAttempts.Remove(loginAttempt);
    }

    public void AddSession(string refreshTokenHash, string ipAddress)
    {
        Sessions.Add(Session.Create(refreshTokenHash, ipAddress));
    }

    public IdentityRequestPasswordResetResult RequestPasswordReset()
    {
        var token = AlphanumericRandomStringGenerator.GenerateAlphanumericToken();

        var tokenHash = StringHasher.Hash(token);

        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenValidTo = Clock.Now + DomainConstants.PasswordResetTokenDuration;

        return new IdentityRequestPasswordResetResult(token);
    }

    public void ResetPassword(string newPassword)
    {
        if (PasswordResetTokenValidTo < Clock.Now)
        {
            throw new DomainException(DomainExceptionMessages.TokenExpired);
        }
        
        PasswordHash = PasswordHasher.Hash(newPassword);
        PasswordResetTokenHash =  null;
        PasswordResetTokenValidTo = null;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmationTokenValidTo < Clock.Now)
        {
            throw new DomainException(DomainExceptionMessages.TokenExpired);
        }

        if (EmailConfirmed)
        {
            throw new DomainException(DomainExceptionMessages.EmailAlreadyConfirmed);
        }

        EmailConfirmed = true;
        EmailConfirmationTokenHash = null;
        EmailConfirmationTokenValidTo = null;
    }
}
