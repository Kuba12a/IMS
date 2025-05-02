using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using Common.Utils.Security;
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
using Platform.Domain.Repositories;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityRefreshTokenCommandValidator : AbstractValidator<IdentityRefreshTokenCommand>
{
    public IdentityRefreshTokenCommandValidator()
    {
    }
}

public class IdentityRefreshTokenCommand : IRequest<IdentityTokensViewModel>
{

    public IdentityRefreshTokenCommand()
    {
    }
}

internal class IdentityRefreshTokenCommandHandler : IRequestHandler<IdentityRefreshTokenCommand, IdentityTokensViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;
    private readonly ISecurityTokenService _securityTokenService;

    public IdentityRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository,
        IHttpContextAccessor httpContextAccessor, ICookieService cookieService, ISecurityTokenService securityTokenService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _httpContextAccessor = httpContextAccessor;
        _cookieService = cookieService;
        _securityTokenService = securityTokenService;
    }

    public async Task<IdentityTokensViewModel> Handle(IdentityRefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var refreshToken = _cookieService.GetCookie(AuthConstants.RefreshTokenCookieName);
        
        if (refreshToken == null)
        {
            throw new AuthorizationException();
        }
        
        var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        var refreshTokenHash = StringHasher.Hash(refreshToken);

        var identity = await _identityRepository
            .FirstOrDefaultByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);
        
        if (identity == null)
        {
            throw new AuthorizationException();
        }
        
        var idToken = _securityTokenService.CreateIdToken(identity.Id, identity.Name);
        var accessToken = _securityTokenService.CreateAccessToken(identity.Id);
        var newRefreshToken = _securityTokenService.CreateRefreshToken(identity.Id);
        
        identity.RefreshSession(refreshTokenHash, ipAddress, StringHasher.Hash(newRefreshToken.Value));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _cookieService.SetCookie(AuthConstants.AccessTokenCookieName,accessToken.Value,
            accessToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = accessToken.ExpiresAt,
                Path = "/"
            });
        
        _cookieService.SetCookie(AuthConstants.RefreshTokenCookieName, newRefreshToken.Value,
            newRefreshToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresAt,
                Path = "/Identity/refresh-token"
            });
        
        _cookieService.SetCookie(AuthConstants.RefreshTokenCookieName, newRefreshToken.Value,
            newRefreshToken.ExpiresAt, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresAt,
                Path = "/Identity/logout"
            });
        
        return new IdentityTokensViewModel(idToken.Value);
    }
}
