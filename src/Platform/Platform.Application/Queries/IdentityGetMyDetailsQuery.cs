using Common.Application.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Services.Auth;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Queries;

public class IdentityGetMyDetailsQueryValidator : AbstractValidator<IdentityGetMyDetailsQuery>
{
    public IdentityGetMyDetailsQueryValidator()
    {
    }
}

public class IdentityGetMyDetailsQuery : IRequest<IdentityDetailsViewModel>
{
    
    public IdentityGetMyDetailsQuery()
    {
    }
}

internal class IdentityGetMyDetailsQueryHandler : IRequestHandler<IdentityGetMyDetailsQuery, IdentityDetailsViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;
    private readonly IAuthenticationContextService _authenticationContextService;

    public IdentityGetMyDetailsQueryHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository, 
        IAuthenticationContextService authenticationContextService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _authenticationContextService = authenticationContextService;
    }

    public async Task<IdentityDetailsViewModel> Handle(IdentityGetMyDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var identityId = _authenticationContextService.RequireContext().IdentityId;
        
        var identity = await _identityRepository
            .FirstOrDefaultByIdAsync(identityId, cancellationToken);

        if (identity == default)
        {
            throw new AuthorizationException();
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new IdentityDetailsViewModel(identity.Id, identity.Email, identity.Name);
    }
}
