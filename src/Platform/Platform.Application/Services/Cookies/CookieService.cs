using Microsoft.AspNetCore.Http;

namespace Platform.Application.Services.Cookies;

public interface ICookieService
{
    void SetCookie(string key, string value, DateTimeOffset expiresAt, CookieOptions? options = null);
    string? GetCookie(string key);
    void DeleteCookie(string key, CookieOptions? cookieOptions = null);
}

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public void SetCookie(string key, string value, DateTimeOffset expiresAt, CookieOptions? options = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        options ??= new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
    }

    public string? GetCookie(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(key, out var value) ? value : null;
    }

    public void DeleteCookie(string key, CookieOptions? cookieOptions = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (cookieOptions != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key, cookieOptions);
        }
        else
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }
    }
}
