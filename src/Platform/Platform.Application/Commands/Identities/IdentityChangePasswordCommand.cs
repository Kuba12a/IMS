using Common.Application.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.Validators;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityChangePasswordCommandValidator : AbstractValidator<IdentityChangePasswordCommand>
{
    public IdentityChangePasswordCommandValidator()
    {
        RuleFor(command => command.Password).NotEmpty();
        RuleFor(command => command.NewPassword).NotEmpty().Password();
    }
}

public class IdentityChangePasswordCommand : IRequest<SuccessResultViewModel>
{
    public string Password { get; }
    public string NewPassword { get; }
    
    public IdentityChangePasswordCommand(string password, string newPassword)
    {
        Password = password;
        NewPassword = newPassword;
    }
}

internal class IdentityChangePasswordCommandHandler : IRequestHandler<IdentityChangePasswordCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IAuthenticationContextService _authenticationContextService;
    private readonly PasswordSettings _passwordSettings;

    public IdentityChangePasswordCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository, 
        IAuthenticationContextService authenticationContextService, PasswordSettings passwordSettings)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _authenticationContextService = authenticationContextService;
        _passwordSettings = passwordSettings;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var identityId = _authenticationContextService.RequireContext().IdentityId;
        
        var identity = await _identityRepository
            .FirstOrDefaultByIdAsync(identityId, cancellationToken);

        if (identity == default)
        {
            throw new AuthorizationException();
        }
        
        identity.ChangePassword(command.Password, command.NewPassword, _passwordSettings.Pepper);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
