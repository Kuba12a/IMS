using FluentValidation;
using Platform.Application.Constants;

namespace Platform.Application.Validators;

public static class ValidationRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(AuthConstants.MinPasswordLength)
            .WithMessage("{PropertyName} must contain at least " + AuthConstants.MinPasswordLength + " characters")
            .MaximumLength(AuthConstants.MaxPasswordLength)
            .WithMessage("{PropertyName} must contain max " + AuthConstants.MaxPasswordLength + " characters")
            .Matches($@"[0-9]+")
            .WithMessage("{PropertyName} must contain at least one number")
            .Matches(@"[A-Z]+")
            .WithMessage("{PropertyName} must contain at least one uppercase character")
            .Matches(@"[a-z]+")
            .WithMessage("{PropertyName} must contain at least one lowercase character")
            .Matches(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]+")
            .WithMessage("{PropertyName} must contain at least one special character");
    }
}
