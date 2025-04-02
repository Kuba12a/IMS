namespace Platform.Application.Services.Auth;

public record TokenContext(Guid Id, DateTime ExpirationTime);
