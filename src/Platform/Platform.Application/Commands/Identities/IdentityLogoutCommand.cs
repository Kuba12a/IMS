using Common.Application.Exceptions;
using Common.Utils.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Platform.Application.Constants;
using Platform.Application.InfrastructureInterfaces;
using Platform.Application.Services.Cookies;
using Platform.Application.ViewModels;
using Platform.Domain.Repositories;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityLogoutCommandValidator : AbstractValidator<IdentityLogoutCommand>
{
    public IdentityLogoutCommandValidator()
    {
    }
}

public class IdentityLogoutCommand : IRequest<SuccessResultViewModel>
{
    
    public IdentityLogoutCommand()
    {
    }
}

internal class IdentityLogoutCommandHandler : IRequestHandler<IdentityLogoutCommand, SuccessResultViewModel>
{
    private readonly ICookieService _cookieService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IdentityLogoutCommandHandler(ICookieService cookieService, IIdentityRepository identityRepository,
        IUnitOfWork unitOfWork)
    {
        _cookieService = cookieService;
        _identityRepository = identityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityLogoutCommand command,
        CancellationToken cancellationToken)
    {
        var refreshToken = _cookieService.GetCookie(AuthConstants.RefreshTokenCookieName);

        if (refreshToken == null)
        {
            throw new LogicException("No refresh token attached");
        }

        var refreshTokenHash = StringHasher.Hash(refreshToken);

        var identity = await _identityRepository
            .FirstOrDefaultByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);

        identity?.RemoveSession(refreshTokenHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cookieService.DeleteCookie(AuthConstants.AccessTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Domain = ".example.localhost"
        });
        _cookieService.DeleteCookie(AuthConstants.RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/Identity/refresh-token",
            Domain = ".example.localhost"
        });
        
        _cookieService.DeleteCookie(AuthConstants.RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/Identity/logout",
            Domain = ".example.localhost"
        });
        
        return new SuccessResultViewModel(true);
    }
}
