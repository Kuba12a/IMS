using Common.Types.Settings;
using FluentValidation;

namespace Platform.Application.Services.UrlBuilder;

internal class UrlBuilderSettingsValidator : AbstractValidator<UrlBuilderSettings>
{
    public UrlBuilderSettingsValidator()
    {
        RuleFor(settings => settings.FrontendUrl).NotEmpty();
    }
}

public class UrlBuilderSettings : IValidatable
{
    public string FrontendUrl { get; set; }
    
    public void Validate()
    {
        var validator = new UrlBuilderSettingsValidator();
        validator.ValidateAndThrow(this);
    }
}
