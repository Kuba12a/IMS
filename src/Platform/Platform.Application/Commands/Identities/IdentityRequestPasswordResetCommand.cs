using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Constants;
using Platform.Application.Services.RateLimit;
using Platform.Application.Services.UrlBuilder;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityRequestPasswordResetCommandValidator : AbstractValidator<IdentityRequestPasswordResetCommand>
{
    public IdentityRequestPasswordResetCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty();
    }
}

public class IdentityRequestPasswordResetCommand : IRequest<SuccessResultViewModel>
{
    public string Email { get; }
    
    public IdentityRequestPasswordResetCommand(string email)
    {
        Email = email;
    }
}

internal class IdentityRequestPasswordResetCommandHandler : IRequestHandler<IdentityRequestPasswordResetCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IEmailGateway _emailGateway;
    private readonly IRateLimitService _rateLimitService;
    private readonly IUrlBuilderService _urlBuilderService;

    public IdentityRequestPasswordResetCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository,
        IEmailGateway emailGateway, IRateLimitService rateLimitService, IUrlBuilderService urlBuilderService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _emailGateway = emailGateway;
        _rateLimitService = rateLimitService;
        _urlBuilderService = urlBuilderService;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityRequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        await _rateLimitService.TryIncrementAttemptCounterAsync(
            keyPrefix: AuthConstants.RequestPasswordResetAttemptCountKeyPrefix,
            identifier: command.Email,
            message: AuthConstants.RequestPasswordResetAttemptExceptionMessage,
            attemptLimit: AuthConstants.RequestPasswordResetAttemptLimit,
            keyExpiresIn: AuthConstants.RequestPasswordResetAttemptLockDuration
        );
        
        var identity = await _identityRepository
            .FirstOrDefaultByEmailAsync(command.Email, cancellationToken);

        if (identity == default)
        {
            return new SuccessResultViewModel(true);
        }

        var requestResetPasswordResult = identity.RequestPasswordReset();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendPasswordResetEmail(identity.Email, requestResetPasswordResult.Token,
            cancellationToken);
        
        return new SuccessResultViewModel(true);
    }

    private async Task SendPasswordResetEmail(string sendTo, string passwordResetToken, CancellationToken cancellationToken)
    {
        var passwordResetUrl = _urlBuilderService.BuildResetPasswordUrl(passwordResetToken);
        
        await _emailGateway.SendEmailAsync("Password Reset",
            $"Reset your password: {passwordResetUrl}",
            sendTo,
            cancellationToken);
    }
}
