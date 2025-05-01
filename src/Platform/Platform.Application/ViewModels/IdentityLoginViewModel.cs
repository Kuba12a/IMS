namespace Platform.Application.ViewModels;

public sealed record IdentityLoginViewModel(string IdToken, string AccessToken, string RefreshToken);
