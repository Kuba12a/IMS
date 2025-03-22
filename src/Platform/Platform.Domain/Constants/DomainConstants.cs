namespace Platform.Domain.Constants;

public static class DomainConstants
{
    public static readonly TimeSpan EmailConfirmationTokenDuration = TimeSpan.FromMinutes(15);
    public const int EmailConfirmationTokenLength = 64;
    public static readonly TimeSpan PasswordResetTokenDuration = TimeSpan.FromMinutes(15);
    public const int PasswordResetTokenLength = 64;
}
