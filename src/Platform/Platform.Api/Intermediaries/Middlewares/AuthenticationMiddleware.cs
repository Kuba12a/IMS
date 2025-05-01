using Microsoft.AspNetCore.Mvc.Filters;
using Platform.Api.Intermediaries.Utils.Extensions;
using Platform.Application.Services.Auth;

namespace Platform.Api.Intermediaries.Middlewares;

public class AuthenticationContextAuthorizationFilter : IAuthorizationFilter
{
    private readonly IAuthenticationContextService _authenticationContextService;

    public AuthenticationContextAuthorizationFilter(IAuthenticationContextService authenticationContextService)
    {
        _authenticationContextService = authenticationContextService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var claims = context.HttpContext.User?.Claims?.ToArray();
        var id = claims?.GetUserId();
        
        if (id.HasValue)
        {
            _authenticationContextService.Context = new AuthenticationContext
            {
                IdentityId = id.Value,
            };
        }
    }
}
