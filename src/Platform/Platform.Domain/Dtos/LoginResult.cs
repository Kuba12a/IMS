using Platform.Domain.Models.Identities;
using Platform.Domain.Types;

namespace Platform.Domain.Dtos;

public record LoginResult(Identity Identity, TokenResult IdToken, TokenResult AccessToken, TokenResult RefreshToken);
