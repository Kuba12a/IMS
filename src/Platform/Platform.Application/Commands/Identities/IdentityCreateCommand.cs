using Common.Application.Exceptions;
using FluentValidation;
using Platform.Application.InfrastructureInterfaces;
using Platform.Domain.Models.Identities;
using Platform.Domain.Repositories;
using MediatR;
using Platform.Application.Services.UrlBuilder;
using Platform.Application.Validators;
using Platform.Application.ViewModels;

#pragma warning disable CS8620

// ReSharper disable ClassNeverInstantiated.Global

namespace Platform.Application.Commands.Identities;

public class IdentityCreateCommandValidator : AbstractValidator<IdentityCreateCommand>
{
    public IdentityCreateCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Password).NotEmpty().Password();
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
    private readonly IEmailGateway _emailGateway;
    private readonly PasswordSettings _passwordSettings;
    private readonly IUrlBuilderService _urlBuilderService;

    public IdentityCreateCommandHandler(IUnitOfWork unitOfWork, IIdentityRepository identityRepository,
        IEmailGateway emailGateway, PasswordSettings passwordSettings, IUrlBuilderService urlBuilderService)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
        _emailGateway = emailGateway;
        _passwordSettings = passwordSettings;
        _urlBuilderService = urlBuilderService;
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

        var identityCreateResult = Identity.Create(command.Email, command.Name, command.Password, _passwordSettings.Pepper);
        
        await _identityRepository.AddAsync(identityCreateResult.Identity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendWelcomeEmail(identityCreateResult.Identity.Email, identityCreateResult.ConfirmationToken,
            cancellationToken);
        
        return new SuccessResultViewModel(true);
    }

    private async Task SendWelcomeEmail(string sendTo, string confirmationToken, CancellationToken cancellationToken)
    {
        var confirmEmailUrl = _urlBuilderService.BuildConfirmEmailUrl(confirmationToken);

        await _emailGateway.SendEmailAsync("Welcome",
            $"Please confirm your account: {confirmEmailUrl}",
            sendTo,
            cancellationToken);
    }
}
