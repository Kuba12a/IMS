using Platform.Domain.Models.Identities;

namespace Platform.Domain.Dtos;

public record IdentityCreateResult(Identity Identity, string ConfirmationToken);
