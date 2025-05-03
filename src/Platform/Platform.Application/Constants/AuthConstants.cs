namespace Platform.Application.Constants;

public static class AuthConstants
{
    public const string IdentityIdClaim = "identityId";
    public const string TokenTypeClaim = "type";
    public const string IdentityNameClaim = "identityName";

    public const string TwoFactorSessionCookieName = "TFSCookie";
    
    public const string AccessTokenCookieName = "AccessToken";
    public const string RefreshTokenCookieName = "RefreshToken";
    
    public const string LoginAttemptExceptionMessage = "Too many failed login attempts";
    public const string LoginAttemptCountKeyPrefix = "login_attempt_count";
    public const int LoginAttemptLimit = 5;
    public static readonly TimeSpan LoginAttemptLockDuration = TimeSpan.FromMinutes(5);
    
    public const string RequestPasswordResetAttemptExceptionMessage = "Too many requests to reset password";
    public const string RequestPasswordResetAttemptCountKeyPrefix = "request_password_reset_attempt_count";
    public const int RequestPasswordResetAttemptLimit = 1;
    public static readonly TimeSpan RequestPasswordResetAttemptLockDuration = TimeSpan.FromMinutes(5);

    public const int MinPasswordLength = 13;
    public const int MaxPasswordLength = 20;
}
