using Platform.Domain.Common;
using Platform.Domain.Models.Identities;

namespace Platform.Domain.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Task<Identity?> FirstOrDefaultByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Identity?> FirstOrDefaultByAuthCodeAsync(string authCode, CancellationToken cancellationToken = default);
}
