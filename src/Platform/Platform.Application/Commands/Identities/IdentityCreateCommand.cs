using Common.Application.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Models.Identities;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityCreateCommandValidator : AbstractValidator<IdentityCreateCommand>
{
    public IdentityCreateCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
        RuleFor(command => command.Email).NotEmpty();
        RuleFor(command => command.Password).NotEmpty();
    }
}

public class IdentityCreateCommand : IRequest<SuccessResultViewModel>
{
    public string Email { get; }
    public string Password { get; }
    public string Name { get; }
    
    public IdentityCreateCommand(string email, string password, string name)
    {
        Email = email;
        Password = password;
        Name = name;
    }
}

internal class IdentityCreateCommandHandler : IRequestHandler<IdentityCreateCommand, SuccessResultViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public IdentityCreateCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<SuccessResultViewModel> Handle(IdentityCreateCommand command,
        CancellationToken cancellationToken)
    {
        var existingIdentity = await _identityRepository
            .FirstOrDefaultByEmailAsync(command.Email, cancellationToken);

        if (existingIdentity != default)
        {
            throw new LogicException("Identity with given email already exists");
        }

        var identity = Identity.Create(command.Email, command.Password);
        
        await _identityRepository.AddAsync(identity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResultViewModel(true);
    }
}
