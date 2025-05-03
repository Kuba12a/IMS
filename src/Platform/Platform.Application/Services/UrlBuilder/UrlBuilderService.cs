namespace Platform.Application.Services.UrlBuilder;

public interface IUrlBuilderService
{
    string BuildResetPasswordUrl(string token);
    string BuildConfirmEmailUrl(string token);
}

public class UrlBuilderService : IUrlBuilderService
{
    private readonly UrlBuilderServiceSettings _urlBuilderServiceSettings;

    public UrlBuilderService(UrlBuilderServiceSettings urlBuilderServiceSettings)
    {
        _urlBuilderServiceSettings = urlBuilderServiceSettings;
    }

    public string BuildResetPasswordUrl(string token)
    {
        return $"{_urlBuilderServiceSettings.FrontendResetPasswordUrl}?token={token}";
    }
    
    public string BuildConfirmEmailUrl(string token)
    {
        return $"{_urlBuilderServiceSettings.FrontendConfirmEmailUrl}?token={token}";
    }
}
