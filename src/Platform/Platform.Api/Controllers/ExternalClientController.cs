using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Queries.ExternalClient;
using Platform.Application.ViewModels;

namespace Platform.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExternalClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExternalClientController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("get-auth-configuration")]
    public Task<AuthConfigurationViewModel> CreateAsync([FromQuery]GetAuthConfigurationQuery query,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(query, cancellationToken);
    }
    
    [HttpPost("get-token-info")]
    public Task<TokenInfoViewModel> InitiateLoginAsync(GetTokenInfoQuery query,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(query, cancellationToken);
    }
}
