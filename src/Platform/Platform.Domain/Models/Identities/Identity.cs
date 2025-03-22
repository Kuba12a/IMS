using Common.Utils;
using Common.Utils.Security;
using Platform.Domain.Common;
using Platform.Domain.Constants;

namespace Platform.Domain.Models.Identities;

public class Identity : IAggregate
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
    
    public static Identity Create(string email, string password)
    {
        var emailConfirmationToken = AlphanumericRandomStringGenerator
            .GenerateAlphanumericToken(DomainConstants.EmailConfirmationTokenLength);

        return new Identity
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower(),
            PasswordHash = PasswordHasher.Hash(password),
            CreatedAt = Clock.Now,
            UpdatedAt = Clock.Now,
            EmailConfirmed = false,
            EmailConfirmationTokenHash = TokenHasher.Hash(emailConfirmationToken),
            EmailConfirmationTokenValidTo = Clock.Now + DomainConstants.EmailConfirmationTokenDuration,
        };
    }
}
