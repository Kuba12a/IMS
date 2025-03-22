using FluentValidation;

namespace Common.Types.Settings;

internal class PostgresSettingsValidator : AbstractValidator<PostgresSettings>
{
    public PostgresSettingsValidator()
    {
        RuleFor(settings => settings.ConnectionString).NotEmpty()
            .When(settings => settings.UrlEnvironmentVariableName == null);
        RuleFor(settings => settings.UrlEnvironmentVariableName).NotEmpty()
            .When(settings => settings.ConnectionString == null);
        RuleFor(settings => settings.UrlEnvironmentVariableName).Null()
            .When(settings => settings.ConnectionString != null);
        RuleFor(settings => settings.ConnectionString).Null()
            .When(settings => settings.UrlEnvironmentVariableName != null);
    }
}

public class PostgresSettings : IValidatable
{
    public string? ConnectionString { get; set; }
    public string? UrlEnvironmentVariableName { get; set; }

    public void Validate()
    {
        var validator = new PostgresSettingsValidator();
        validator.ValidateAndThrow(this);
    }

    public string GetConnectionString()
    {
        if (ConnectionString != null)
        {
            return ConnectionString;
        }

        throw new Exception("DbConnection string can not be null");
    }
}
