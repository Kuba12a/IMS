using Common.Types.Settings;
using FluentValidation;

namespace Platform.Infrastructure.Gateways.Smtp;

internal class SmtpSettingsValidator : AbstractValidator<SmtpSettings>
{
    public SmtpSettingsValidator()
    {
        RuleFor(settings => settings.FromEmail).NotEmpty();
        RuleFor(settings => settings.FromName).NotEmpty();
        RuleFor(settings => settings.Host).NotEmpty();
        RuleFor(settings => settings.Port).NotNull();
        RuleFor(settings => settings.Login).NotEmpty();
        RuleFor(settings => settings.Password).NotEmpty();
    }
}

public class SmtpSettings : IValidatable
{
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    
    public void Validate()
    {
        var validator = new SmtpSettingsValidator();
        validator.ValidateAndThrow(this);
    }
}
