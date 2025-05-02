using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Application.Constants;
using Platform.Application.Services.Auth;
using Platform.Application.Services.Cookies;
using Platform.Application.ViewModels;
using Platform.Domain.Constants;
using Platform.Domain.DomainServices;
using Platform.Domain.Dtos;

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

public class IdentityLoginCommand : IRequest<IdentityTokensViewModel>
{
    public string AuthCode { get; }
    public string CodeVerifier { get; }

    public IdentityLoginCommand(string authCode, string codeVerifier)
    {
        AuthCode = authCode;
        CodeVerifier = codeVerifier;
    }
}

internal class IdentityLoginCommandHandler : IRequestHandler<IdentityLoginCommand, IdentityTokensViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityDomainService _identityDomainService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;

    public IdentityLoginCommandHandler(IUnitOfWork unitOfWork, IIdentityDomainService identityDomainService,
        IHttpContextAccessor httpContextAccessor, ICookieService cookieService)
    {
        _unitOfWork = unitOfWork;
        _identityDomainService = identityDomainService;
        _httpContextAccessor = httpContextAccessor;
        _cookieService = cookieService;
    }

    public async Task<IdentityTokensViewModel> Handle(IdentityLoginCommand command,
        CancellationToken cancellationToken)
    {
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        
        LoginResult loginResult;
        try
        {
            loginResult = await _identityDomainService
                .Login(
                    command.AuthCode,
                    command.CodeVerifier,
                    ipAddress,
                    cancellationToken);
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
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _cookieService.SetCookie(AuthConstants.AccessTokenCookieName, loginResult.AccessToken.Value,
            loginResult.AccessToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginResult.AccessToken.ExpiresAt,
                Path = "/",
                Domain = "localhost"
            });
        
        _cookieService.SetCookie(AuthConstants.RefreshTokenCookieName, loginResult.RefreshToken.Value,
            loginResult.RefreshToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginResult.RefreshToken.ExpiresAt,
                Path = "/Identity/refresh-token",
                Domain = "localhost"
            });
        
        _cookieService.SetCookie(AuthConstants.RefreshTokenCookieName, loginResult.RefreshToken.Value,
            loginResult.RefreshToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginResult.RefreshToken.ExpiresAt,
                Path = "/Identity/logout",
                Domain = "localhost"
            });
        
        return new IdentityTokensViewModel(loginResult.IdToken.Value);
    }
}
