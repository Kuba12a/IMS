using Common.Application.Exceptions;
using Common.Utils.Security;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Constants;
using Platform.Application.Services.Cookies;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityConfirmTwoFactorEmailCodeCommandValidator : AbstractValidator<IdentityConfirmTwoFactorEmailCodeCommand>
{
    public IdentityConfirmTwoFactorEmailCodeCommandValidator()
    {
        RuleFor(command => command.EmailCode).NotEmpty();
    }
}

public class IdentityConfirmTwoFactorEmailCodeCommand : IRequest<IdentityConfirmTwoFactorEmailCodeResultViewModel>
{
    public string EmailCode { get; }
    
    public IdentityConfirmTwoFactorEmailCodeCommand(string emailCode)
    {
        EmailCode = emailCode;
    }
}

internal class IdentityConfirmTwoFactorEmailCodeCommandHandler : IRequestHandler<IdentityConfirmTwoFactorEmailCodeCommand,
    IdentityConfirmTwoFactorEmailCodeResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly ICookieService _cookieService;

    public IdentityConfirmTwoFactorEmailCodeCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository,
        ICookieService cookieService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _cookieService = cookieService;
    }

    public async Task<IdentityConfirmTwoFactorEmailCodeResultViewModel> Handle(IdentityConfirmTwoFactorEmailCodeCommand command,
        CancellationToken cancellationToken)
    {
        var sessionToken = _cookieService.GetCookie(AuthConstants.TwoFactorSessionCookieName);

        if (sessionToken == default)
        {
            throw new LogicException("Session token not valid");
        }
        
        var sessionTokenHash = StringHasher.Hash(sessionToken);
        
        var identity = await _identityRepository
            .FirstOrDefaultBySessionTokenHashAsync(sessionTokenHash, cancellationToken);
        
        if (identity == default)
        {
            throw new LogicException("Session token not valid");
        }
        
        var emailCodeHash = StringHasher.Hash(command.EmailCode);

        var authCode = identity
            .CheckTwoFactorEmailChallenge(sessionTokenHash, emailCodeHash);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new IdentityConfirmTwoFactorEmailCodeResultViewModel(authCode);
    }
}
