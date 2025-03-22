using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Commands.Identities;
using Platform.Application.ViewModels;

namespace Platform.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("create")]
    public Task<SuccessResultViewModel> CreateAsync(IdentityCreateCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
}
