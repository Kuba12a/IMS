using Platform.Domain.Common;

namespace Platform.Domain.Models.Identities;

public record AuthCodeChallenge(string HashedCode, DateTimeOffset CreatedAt, DateTimeOffset ValidTo)
    : ValueObject;
