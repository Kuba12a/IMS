using Common.Application.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentitySetMfaRequiredCommandValidator : AbstractValidator<IdentitySetMfaRequiredCommand>
{
    public IdentitySetMfaRequiredCommandValidator()
    {
        RuleFor(command => command.MfaRequired).NotNull();
    }
}

public class IdentitySetMfaRequiredCommand : IRequest<SuccessResultViewModel>
{
    public bool MfaRequired { get; }
    
    public IdentitySetMfaRequiredCommand(bool mfaRequired)
    {
        MfaRequired = mfaRequired;
    }
}

internal class IdentitySetMfaRequiredCommandHandler : IRequestHandler<IdentitySetMfaRequiredCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IAuthenticationContextService _authenticationContextService;

    public IdentitySetMfaRequiredCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository, 
        IAuthenticationContextService authenticationContextService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _authenticationContextService = authenticationContextService;
    }

    public async Task<SuccessResultViewModel> Handle(IdentitySetMfaRequiredCommand command,
        CancellationToken cancellationToken)
    {
        var identityId = _authenticationContextService.RequireContext().IdentityId;
        
        var identity = await _identityRepository
            .FirstOrDefaultByIdAsync(identityId, cancellationToken);

        if (identity == default)
        {
            throw new AuthorizationException();
        }
        
        identity.SetMfaRequired(command.MfaRequired);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
