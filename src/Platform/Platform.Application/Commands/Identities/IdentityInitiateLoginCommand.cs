using Common.Application.Exceptions;
using Common.Domain.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.ViewModels;
using Platform.Domain.Constants;
using Platform.Domain.Dtos;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityInitiateLoginCommandValidator : AbstractValidator<IdentityInitiateLoginCommand>
{
    public IdentityInitiateLoginCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty();
        RuleFor(command => command.Password).NotEmpty();
        RuleFor(command => command.CodeChallenge).NotEmpty();
    }
}

public class IdentityInitiateLoginCommand : IRequest<IdentityInitiateLoginViewModel>
{
    public string Email { get; }
    public string Password { get; }
    public string CodeChallenge { get; }
    
    public IdentityInitiateLoginCommand(string email, string password, string codeChallenge)
    {
        Email = email;
        Password = password;
        CodeChallenge = codeChallenge;
    }
}

internal class IdentityInitiateLoginCommandHandler : IRequestHandler<IdentityInitiateLoginCommand, IdentityInitiateLoginViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public IdentityInitiateLoginCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<IdentityInitiateLoginViewModel> Handle(IdentityInitiateLoginCommand command,
        CancellationToken cancellationToken)
    {
        var identity = await _identityRepository
            .FirstOrDefaultByEmailAsync(command.Email, cancellationToken);

        if (identity == default)
        {
            throw new AuthenticationException("Invalid credentials");
        }

        InitiateLoginResult initiateLoginResult;
        
        try
        {
            initiateLoginResult = identity.InitiateLogin(command.Password, command.CodeChallenge);
        }
        catch (DomainException exception)
        {
            throw exception.Message switch
            {
                DomainExceptionMessages.InvalidCredentials => new AuthenticationException(exception.Message),
                DomainExceptionMessages.EmailNotConfirmed => new AuthenticationException(exception.Message),
                _ => new LogicException(exception.Message)
            };
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new IdentityInitiateLoginViewModel(initiateLoginResult.AuthCode);
    }
}
