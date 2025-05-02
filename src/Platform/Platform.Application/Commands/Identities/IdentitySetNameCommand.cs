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

public class IdentitySetNameCommandValidator : AbstractValidator<IdentitySetNameCommand>
{
    public IdentitySetNameCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
    }
}

public class IdentitySetNameCommand : IRequest<SuccessResultViewModel>
{
    public string Name { get; }
    
    public IdentitySetNameCommand(string name)
    {
        Name = name;
    }
}

internal class IdentitySetNameCommandHandler : IRequestHandler<IdentitySetNameCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IAuthenticationContextService _authenticationContextService;

    public IdentitySetNameCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository, 
        IAuthenticationContextService authenticationContextService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _authenticationContextService = authenticationContextService;
    }

    public async Task<SuccessResultViewModel> Handle(IdentitySetNameCommand command,
        CancellationToken cancellationToken)
    {
        var identityId = _authenticationContextService.RequireContext().IdentityId;
        
        var identity = await _identityRepository
            .FirstOrDefaultByIdAsync(identityId, cancellationToken);

        if (identity == default)
        {
            throw new AuthorizationException();
        }
        
        identity.SetName(command.Name);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
