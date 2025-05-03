using Common.Types.Settings;
using FluentValidation;
using Platform.Application;
using Platform.Application.Services.Auth;
using Platform.Infrastructure.Gateways.Redis;
using Platform.Infrastructure.Gateways.Smtp;

namespace Platform.Api;

public class PlatformSettingsValidator : AbstractValidator<PlatformSettings>
{
    public PlatformSettingsValidator()
    {
        RuleFor(settings => settings.PostgresSettings).NotNull();
        RuleFor(settings => settings.SecurityTokenSettings).NotNull();
        RuleFor(settings => settings.SmtpSettings).NotNull();
        RuleFor(settings => settings.PasswordSettings).NotNull();
        RuleFor(settings => settings.RedisSettings).NotNull();
    }
}

public class PlatformSettings : IValidatable
{
    public PostgresSettings PostgresSettings { get; set; }
    public SecurityTokenSettings SecurityTokenSettings { get; set; }
    public SmtpSettings SmtpSettings { get; set; }
    public PasswordSettings PasswordSettings { get; set; }
    public RedisSettings RedisSettings { get; set; }

    public void Validate()
    {
        new PlatformSettingsValidator().ValidateAndThrow(this);
        PostgresSettings.Validate();
        SecurityTokenSettings.Validate();
        SmtpSettings.Validate();
        PasswordSettings.Validate();
        RedisSettings.Validate();
    }
}
