using Common.Types.Settings;
using FluentValidation;

namespace Platform.Infrastructure.Gateways.Redis;

internal class RedisSettingsValidator : AbstractValidator<RedisSettings>
{
    public RedisSettingsValidator()
    {
        RuleFor(settings => settings.ConnectionString).NotEmpty();
    }
}

public class RedisSettings : IValidatable
{
    public string ConnectionString { get; set; }
    
    public void Validate()
    {
        var validator = new RedisSettingsValidator();
        validator.ValidateAndThrow(this);
    }
}
