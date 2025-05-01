using Platform.Domain.Models.Identities;

namespace Platform.Domain.Dtos;

public record IdentityCreateLoginAttemptResult(LoginAttempt LoginAttempt, string AuthCode);
