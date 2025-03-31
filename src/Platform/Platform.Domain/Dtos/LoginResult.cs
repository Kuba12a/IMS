using Platform.Domain.Models.Identities;

namespace Platform.Domain.Dtos;

public record LoginResult(Identity Identity, string IdToken, string AccessToken, string RefreshToken);
