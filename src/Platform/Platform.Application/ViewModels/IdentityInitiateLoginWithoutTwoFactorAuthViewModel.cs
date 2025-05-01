namespace Platform.Application.ViewModels;

public record IdentityInitiateLoginWithoutTwoFactorAuthViewModel(string AuthCode) : IdentityInitiateLoginViewModel(false);
