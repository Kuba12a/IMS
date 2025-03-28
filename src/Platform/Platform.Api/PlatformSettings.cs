using Common.Types.Settings;
using FluentValidation;
using Platform.Application.Services.Auth;

namespace Platform.Api;

public class PlatformSettingsValidator : AbstractValidator<PlatformSettings>
{
    public PlatformSettingsValidator()
    {
        RuleFor(settings => settings.PostgresSettings).NotNull();
        RuleFor(settings => settings.SecurityTokenSettings).NotNull();
    }
}

public class PlatformSettings : IValidatable
{
    public PostgresSettings PostgresSettings { get; set; }
    public SecurityTokenSettings SecurityTokenSettings { get; set; }

    public void Validate()
    {
        new PlatformSettingsValidator().ValidateAndThrow(this);
        PostgresSettings.Validate();
        SecurityTokenSettings.Validate();
    }
}
