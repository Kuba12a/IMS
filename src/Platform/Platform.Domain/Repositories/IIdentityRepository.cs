using Platform.Domain.Common;
using Platform.Domain.Models.Identities;

namespace Platform.Domain.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Task<Identity?> FirstOrDefaultAsync(string email, CancellationToken cancellationToken);
}
