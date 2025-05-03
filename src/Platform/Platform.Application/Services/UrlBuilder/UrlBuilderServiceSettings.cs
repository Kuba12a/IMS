using Common.Types.Settings;
using FluentValidation;

namespace Platform.Application.Services.UrlBuilder;

public class UrlBuilderServiceSettingsValidator : AbstractValidator<UrlBuilderServiceSettings>
{
    public UrlBuilderServiceSettingsValidator()
    {
        RuleFor(settings => settings.FrontendResetPasswordUrl).NotNull();
        RuleFor(settings => settings.FrontendConfirmEmailUrl).NotNull();
    }
}

public class UrlBuilderServiceSettings : IValidatable
{
    public string FrontendResetPasswordUrl { get; set; }
    public string FrontendConfirmEmailUrl { get; set; }

    public void Validate()
    {
        new UrlBuilderServiceSettingsValidator().ValidateAndThrow(this);
    }
}
