namespace Platform.Application.Services.Auth;

public record IdTokenContext(Guid Id, DateTime ExpirationTime) : TokenContext(Id, ExpirationTime);
