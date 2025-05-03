using Common.Types.Settings;
using FluentValidation;
using Platform.Application.Services.Auth;

namespace Platform.Application;

public class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
{
    public PasswordSettingsValidator()
    {
        RuleFor(settings => settings.Pepper).NotNull();
    }
}

public class PasswordSettings : IValidatable
{
    public string Pepper { get; set; }

    public void Validate()
    {
        new PasswordSettingsValidator().ValidateAndThrow(this);
    }
}
