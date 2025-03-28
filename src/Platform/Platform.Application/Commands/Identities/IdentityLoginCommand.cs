using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.ViewModels;
using Platform.Domain.Constants;
using Platform.Domain.DomainServices;
using Platform.Domain.Models.Identities;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityLoginCommandValidator : AbstractValidator<IdentityLoginCommand>
{
    public IdentityLoginCommandValidator()
    {
        RuleFor(command => command.AuthCode).NotEmpty();
        RuleFor(command => command.CodeVerifier).NotEmpty();
    }
}

public class IdentityLoginCommand : IRequest<IdentityLoginViewModel>
{
    public string AuthCode { get; }
    public string CodeVerifier { get; }

    public IdentityLoginCommand(string authCode, string codeVerifier)
    {
        AuthCode = authCode;
        CodeVerifier = codeVerifier;
    }
}

internal class IdentityLoginCommandHandler : IRequestHandler<IdentityLoginCommand, IdentityLoginViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityDomainService _identityDomainService;
    private readonly ISecurityTokenService _securityTokenService;

    public IdentityLoginCommandHandler(IUnitOfWork unitOfWork, IIdentityDomainService identityDomainService,
        ISecurityTokenService securityTokenService)
    {
        _unitOfWork = unitOfWork;
        _identityDomainService = identityDomainService;
        _securityTokenService = securityTokenService;
    }

    public async Task<IdentityLoginViewModel> Handle(IdentityLoginCommand command,
        CancellationToken cancellationToken)
    {
        Identity identity;
        try
        {
            identity = await _identityDomainService
                .Login(command.AuthCode, command.CodeVerifier, cancellationToken);
        }
        catch (DomainException exception)
        {
            throw exception.Message switch
            {
                DomainExceptionMessages.LoginAttemptNotFound => new AuthenticationException(exception.Message),
                DomainExceptionMessages.InvalidCredentials => new AuthenticationException(exception.Message),
                DomainExceptionMessages.EmailNotConfirmed => new AuthenticationException(exception.Message),
                _ => new LogicException(exception.Message)
            };
        }

        var idToken = _securityTokenService.CreateIdToken(identity.Id);
        var accessToken = _securityTokenService.CreateAccessToken(identity.Id);
        var refreshToken = _securityTokenService.CreateRefreshToken(identity.Id);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new IdentityLoginViewModel(idToken, accessToken, refreshToken);
    }
}
