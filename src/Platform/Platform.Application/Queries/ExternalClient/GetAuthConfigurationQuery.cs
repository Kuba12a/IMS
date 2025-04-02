using FluentValidation;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.ViewModels;

namespace Platform.Application.Queries.ExternalClient;

public class GetAuthConfigurationQueryValidator : AbstractValidator<GetAuthConfigurationQuery>
{
    public GetAuthConfigurationQueryValidator()
    {
    }
}

public class GetAuthConfigurationQuery : IRequest<AuthConfigurationViewModel>
{
    
    public GetAuthConfigurationQuery()
    {
    }
}

internal class GetAuthConfigurationQueryHandler : IRequestHandler<GetAuthConfigurationQuery, AuthConfigurationViewModel>
{
    private readonly SecurityTokenSettings _securityTokenSettings;

    public GetAuthConfigurationQueryHandler(SecurityTokenSettings securityTokenSettings)
    {
        _securityTokenSettings = securityTokenSettings;
    }

    public async Task<AuthConfigurationViewModel> Handle(GetAuthConfigurationQuery query, CancellationToken cancellationToken)
    {
        var publicAuthKey = _securityTokenSettings.TokenPublicKey;
        
        return new AuthConfigurationViewModel(publicAuthKey);
    }
}
