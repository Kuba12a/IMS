namespace Platform.Application.Services.UrlBuilder;

public interface IUrlBuilderService
{
    string BuildAccountConfirmationUrl(string token);
    string BuildPasswordResetUrl(string token);
}

public class UrlBuilderService : IUrlBuilderService
{
    private readonly UrlBuilderSettings _urlBuilderSettings;

    public UrlBuilderService(UrlBuilderSettings urlBuilderSettings)
    {
        _urlBuilderSettings = urlBuilderSettings;
    }

    public string BuildAccountConfirmationUrl(string token)
    {
        return $"{_urlBuilderSettings.FrontendUrl}/confirm-email?token={token}";
    }
    
    public string BuildPasswordResetUrl(string token)
    {
        return $"{_urlBuilderSettings.FrontendUrl}/reset-password?token={token}";
    }
}
