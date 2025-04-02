using Common.Application.Exceptions;
using Common.Utils.Security;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityResetPasswordCommandValidator : AbstractValidator<IdentityResetPasswordCommand>
{
    public IdentityResetPasswordCommandValidator()
    {
        RuleFor(command => command.Token).NotEmpty();
        RuleFor(command => command.NewPassword).NotEmpty();
    }
}

public class IdentityResetPasswordCommand : IRequest<SuccessResultViewModel>
{
    public string Token { get; }
    public string NewPassword { get; }
    
    public IdentityResetPasswordCommand(string token, string newPassword)
    {
        Token = token;
        NewPassword = newPassword;
    }
}

internal class IdentityResetPasswordCommandHandler : IRequestHandler<IdentityResetPasswordCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public IdentityResetPasswordCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = StringHasher.Hash(command.Token);

        var identity = await _identityRepository
            .FirstOrDefaultByResetPasswordTokenHashAsync(tokenHash, cancellationToken);

        if (identity == default)
        {
            throw new LogicException("Invalid reset password token");
        }

        identity.ResetPassword(command.NewPassword);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
