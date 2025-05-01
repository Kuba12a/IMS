using Common.Application.Exceptions;

namespace Platform.Application.Services.Auth;

public interface IAuthenticationContextService
{
    public AuthenticationContext? Context { get; set; }
    public AuthenticationContext RequireContext();
}

public class AuthenticationContext
{
    public Guid IdentityId { get; init; }
}

public class AuthenticationContextService : IAuthenticationContextService
{
    public AuthenticationContext? Context { get; set; }

    public AuthenticationContext RequireContext()
    {
        if (Context == null)
        {
            throw new AuthorizationException();
        }

        return Context;
    }
}
