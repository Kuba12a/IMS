namespace Platform.Application.Services.Auth;

public record AccessTokenContext(Guid Id, DateTime ExpirationTime)  : TokenContext(Id, ExpirationTime);
