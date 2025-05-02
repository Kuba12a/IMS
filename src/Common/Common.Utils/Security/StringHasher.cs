using System.Security.Cryptography;
using System.Text;

namespace Common.Utils.Security;

public static class StringHasher
{
    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    
        var base64 = Convert.ToBase64String(bytes);
        return base64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
