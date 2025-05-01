namespace Platform.Domain.Dtos;

public record IdentityInitiateLoginWithEmailCodeResult(string SessionToken, string EmailCode, DateTimeOffset ExpiresAt)
    : IdentityInitiateLoginResult;
