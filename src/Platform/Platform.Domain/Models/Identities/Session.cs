using Common.Utils;
using Platform.Domain.Common;

namespace Platform.Domain.Models.Identities;

public class Session : Entity
{
    public Guid Id { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public string RefreshTokenHash { get; private init; }
    public string IpAddress { get; private init; }

    internal static Session Create(string refreshTokenHash, string ipAddress)
    {
        return new Session()
        {
            Id = Guid.NewGuid(),
            CreatedAt = Clock.Now,
            RefreshTokenHash = refreshTokenHash,
            IpAddress = ipAddress
        };
    }
}
