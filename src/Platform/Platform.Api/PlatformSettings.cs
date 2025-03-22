using Common.Types.Settings;
using FluentValidation;

namespace Platform.Api;

public class PlatformSettingsValidator : AbstractValidator<PlatformSettings>
{
    public PlatformSettingsValidator()
    {
        RuleFor(settings => settings.PostgresSettings).NotNull();
    }
}

public class PlatformSettings : IValidatable
{
    public PostgresSettings PostgresSettings { get; set; }

    public void Validate()
    {
        new PlatformSettingsValidator().ValidateAndThrow(this);
        PostgresSettings.Validate();
    }
}
