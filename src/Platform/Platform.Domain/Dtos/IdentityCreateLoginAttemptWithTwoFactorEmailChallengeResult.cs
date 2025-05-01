using Platform.Domain.Models.Identities;

namespace Platform.Domain.Dtos;

public record IdentityCreateLoginAttemptWithTwoFactorChallengeResult(LoginAttempt LoginAttempt, string SessionToken,
    string EmailCode);
