using Platform.Domain.Common;
using Platform.Domain.Models.Identities;

namespace Platform.Domain.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Task<Identity?> FirstOrDefaultByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Identity?> FirstOrDefaultByAuthCodeHashAsync(string authCodeHash, CancellationToken cancellationToken = default);
    Task<Identity?> FirstOrDefaultByConfirmationTokenHashAsync(string confirmationTokenHash,
        CancellationToken cancellationToken = default);
    Task<Identity?> FirstOrDefaultByResetPasswordTokenHashAsync(string resetPasswordTokenHash,
        CancellationToken cancellationToken = default);
    Task<Identity?> FirstOrDefaultBySessionTokenHashAsync(string sessionTokenHash,
        CancellationToken cancellationToken = default);
}
