namespace Platform.Domain.Dtos;

public record IdentityInitiateLoginWithoutTwoFactorAuthResult(string AuthCode) : IdentityInitiateLoginResult;
