using Platform.Domain.Common;

namespace Platform.Domain.Models.Identities;

public record TwoFactorEmailAuthenticationChallenge(string CodeHash, string SessionTokenHash, DateTimeOffset CreatedAt,
    DateTimeOffset ValidTo) : ValueObject;
