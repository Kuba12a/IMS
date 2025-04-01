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

public class IdentityConfirmEmailCommandValidator : AbstractValidator<IdentityConfirmEmailCommand>
{
    public IdentityConfirmEmailCommandValidator()
    {
        RuleFor(command => command.Token).NotEmpty();
    }
}

public class IdentityConfirmEmailCommand : IRequest<SuccessResultViewModel>
{
    public string Token { get; }
    
    public IdentityConfirmEmailCommand(string token)
    {
        Token = token;
    }
}

internal class IdentityConfirmEmailCommandHandler : IRequestHandler<IdentityConfirmEmailCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public IdentityConfirmEmailCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = StringHasher.Hash(command.Token);

        var identity = await _identityRepository
            .FirstOrDefaultByConfirmationTokenHashAsync(tokenHash, cancellationToken);

        if (identity == default)
        {
            throw new LogicException("Invalid confirmation token");
        }

        identity.ConfirmEmail(tokenHash);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
