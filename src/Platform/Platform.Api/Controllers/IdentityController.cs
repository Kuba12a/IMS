using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("initiate-login")]
    public Task<IdentityInitiateLoginViewModel> InitiateLoginAsync(IdentityInitiateLoginCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [HttpPost("confirm-two-factor-email-code")]
    public Task<IdentityConfirmTwoFactorEmailCodeResultViewModel> ConfirmEmailCodeAsync(IdentityConfirmTwoFactorEmailCodeCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [HttpPost("login")]
    public Task<IdentityLoginViewModel> LoginAsync(IdentityLoginCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [HttpPost("confirm-account")]
    public Task<SuccessResultViewModel> ConfirmAccountAsync(IdentityConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [HttpPost("request-password-reset")]
    public Task<SuccessResultViewModel> RequestPasswordResetAsync(IdentityRequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [HttpPost("reset-password")]
    public Task<SuccessResultViewModel> ResetPasswordAsync(IdentityResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
    
    [Authorize(Policy = "Identities")]
    [HttpPost("set-mfa-required")]
    public Task<SuccessResultViewModel> SetMfaRequiredAsync(IdentitySetMfaRequiredCommand command,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(command, cancellationToken);
    }
}
