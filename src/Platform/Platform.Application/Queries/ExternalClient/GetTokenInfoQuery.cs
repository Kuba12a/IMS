using Common.Application.Exceptions;
using FluentValidation;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.ViewModels;

namespace Platform.Application.Queries.ExternalClient;

public class GetTokenInfoQueryValidator : AbstractValidator<GetTokenInfoQuery>
{
    public GetTokenInfoQueryValidator()
    {
    }
}

public class GetTokenInfoQuery : IRequest<TokenInfoViewModel>
{
    public string Token { get; init; }
    
    public GetTokenInfoQuery(string token)
    {
        Token = token;
    }
}

internal class GetTokenInfoQueryHandler : IRequestHandler<GetTokenInfoQuery, TokenInfoViewModel>
{
    private readonly ISecurityTokenService _securityTokenService;

    public GetTokenInfoQueryHandler(ISecurityTokenService securityTokenService)
    {
        _securityTokenService = securityTokenService;
    }

    public async Task<TokenInfoViewModel> Handle(GetTokenInfoQuery query, CancellationToken cancellationToken)
    {
        var tokenContext = _securityTokenService.GetTokenContext(query.Token);

        if (tokenContext == null)
        {
            throw new AuthenticationException();
        }
        
        return new TokenInfoViewModel(tokenContext.Id, tokenContext.ExpirationTime);
    }
}
