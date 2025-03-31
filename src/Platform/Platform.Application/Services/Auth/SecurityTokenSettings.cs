using System.Text;
using Common.Types.Settings;
using Common.Utils.Security;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Platform.Application.Services.Auth;

public class SecurityTokenSettingsValidator : AbstractValidator<SecurityTokenSettings>
{
    public SecurityTokenSettingsValidator()
    {
        RuleFor(settings => settings.TokenPrivateKey).NotEmpty().MinimumLength(16);
        RuleFor(settings => settings.TokenPublicKey).NotEmpty().MinimumLength(16);
        RuleFor(settings => settings.AccessTokenDurationInSeconds).NotNull().GreaterThan(0);
        RuleFor(settings => settings.IdTokenDurationInSeconds).NotNull().GreaterThan(0);
        RuleFor(settings => settings.RefreshTokenDurationInSeconds).NotNull().GreaterThan(0);
    }
}

public class SecurityTokenSettings : IValidatable
{
    public string TokenPrivateKey { get; set; }
    public string TokenPublicKey { get; set; }
    public int AccessTokenDurationInSeconds { get; set; }
    public int IdTokenDurationInSeconds { get; set; }
    public int RefreshTokenDurationInSeconds { get; set; }

    public void Validate()
    {
        var validator = new SecurityTokenSettingsValidator();
        validator.ValidateAndThrow(this);
    }

    public RsaSecurityKey GetTokenPrivateKey()
    {
        return RsaSecurityKeyHelper.GetRsaSecurityKey(TokenPrivateKey, isPrivateKey: true);
    }
    
    public RsaSecurityKey GetTokenPublicKey()
    {
        return RsaSecurityKeyHelper.GetRsaSecurityKey(TokenPublicKey);
    }
}
