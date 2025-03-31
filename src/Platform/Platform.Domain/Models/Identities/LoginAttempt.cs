using Common.Utils;
using Platform.Domain.Common;

namespace Platform.Domain.Models.Identities;

public class LoginAttempt : Entity
{
    public Guid Id { get; private init; }
    public string AuthCode { get; private init; }
    public string CodeChallenge { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset ValidTo { get; private init; }

    internal static LoginAttempt Create(
        string authCode,
        string codeChallenge,
        DateTimeOffset validTo)
    {
        return new LoginAttempt
        {
            Id = Guid.NewGuid(),
            AuthCode = authCode,
            CodeChallenge = codeChallenge,
            CreatedAt = Clock.Now,
            ValidTo = validTo
        };
    }
}
