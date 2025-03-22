using Platform.Domain.Models.Identities;

namespace Platform.Application.InfrastructureInterfaces;

public interface IReadStorage
{
    IQueryable<Identity> Identities { get; }
}
