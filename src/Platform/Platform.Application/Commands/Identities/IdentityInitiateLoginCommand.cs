using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Constants;
using Platform.Application.Services.Cookies;
using Platform.Application.Services.RateLimit;
using Platform.Application.ViewModels;
using Platform.Domain.Constants;
using Platform.Domain.Dtos;
using Platform.Domain.Models.Identities;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityInitiateLoginCommandValidator : AbstractValidator<IdentityInitiateLoginCommand>
{
    public IdentityInitiateLoginCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty();
        RuleFor(command => command.Password).NotEmpty();
        RuleFor(command => command.CodeChallenge).NotEmpty();
    }
}

public class IdentityInitiateLoginCommand : IRequest<IdentityInitiateLoginViewModel>
{
    public string Email { get; }
    public string Password { get; }
    public string CodeChallenge { get; }
    
    public IdentityInitiateLoginCommand(string email, string password, string codeChallenge)
    {
        Email = email;
        Password = password;
        CodeChallenge = codeChallenge;
    }
}

internal class IdentityInitiateLoginCommandHandler : IRequestHandler<IdentityInitiateLoginCommand, IdentityInitiateLoginViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IEmailGateway _emailGateway;
    private readonly ICookieService _cookieService;
    private readonly PasswordSettings _passwordSettings;
    private readonly IRateLimitService _rateLimitService;

    public IdentityInitiateLoginCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository,
        IEmailGateway emailGateway, ICookieService cookieService, PasswordSettings passwordSettings,
        IRateLimitService rateLimitService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _emailGateway = emailGateway;
        _cookieService = cookieService;
        _passwordSettings = passwordSettings;
        _rateLimitService = rateLimitService;
    }

    public async Task<IdentityInitiateLoginViewModel> Handle(IdentityInitiateLoginCommand command,
        CancellationToken cancellationToken)
    {
        await _rateLimitService.TryIncrementAttemptCounterAsync(
            keyPrefix: AuthConstants.LoginAttemptCountKeyPrefix,
            identifier: command.Email,
            message: AuthConstants.LoginAttemptExceptionMessage,
            attemptLimit: AuthConstants.LoginAttemptLimit,
            keyExpiresIn: AuthConstants.LoginAttemptLockDuration
        );
        
        var identity = await _identityRepository
            .FirstOrDefaultByEmailAsync(command.Email, cancellationToken);

        if (identity == default)
        {
            throw new AuthenticationException("Invalid credentials");
        }

        IdentityInitiateLoginResult initiateLoginResult;
        
        try
        {
            initiateLoginResult = identity.InitiateLogin(command.Password, _passwordSettings.Pepper, command.CodeChallenge);

        }
        catch (DomainException exception)
        {
            throw exception.Message switch
            {
                DomainExceptionMessages.InvalidCredentials => new AuthenticationException(exception.Message),
                DomainExceptionMessages.EmailNotConfirmed => new AuthenticationException(exception.Message),
                _ => new LogicException(exception.Message)
            };
        }
        
        await _rateLimitService.ResetAttemptCounterAsync(
            keyPrefix: AuthConstants.LoginAttemptCountKeyPrefix,
            identifier: command.Email);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await HandleInitiateLoginAsync(initiateLoginResult, identity, cancellationToken);
    }

    private async Task<IdentityInitiateLoginViewModel> HandleInitiateLoginAsync(
        IdentityInitiateLoginResult identityInitiateLoginResult,
        Identity identity,
        CancellationToken cancellationToken)
    {
        switch (identityInitiateLoginResult)
        {
            case IdentityInitiateLoginWithEmailCodeResult result :
                await SendTwoFactorCodeEmail(identity.Email, result.EmailCode, cancellationToken);
                
                _cookieService.SetCookie(AuthConstants.TwoFactorSessionCookieName, result.SessionToken, result.ExpiresAt);
                
                return new IdentityInitiateLoginWithEmailCodeViewModel();
            
            case IdentityInitiateLoginWithoutTwoFactorAuthResult result :
                return new IdentityInitiateLoginWithoutTwoFactorAuthViewModel(result.AuthCode);
            
            default:
                throw new Exception("Login method not supported");
        }
    }
    
    private async Task SendTwoFactorCodeEmail(string sendTo, string emailCode, CancellationToken cancellationToken)
    {
        await _emailGateway.SendEmailAsync("Two Factor code",
            $"Your 2fa code: {emailCode}",
            sendTo,
            cancellationToken);
    }
}
