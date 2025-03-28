using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Common.Utils.Security;

public static class RsaSecurityKeyHelper
{
    public static RsaSecurityKey GetRsaSecurityKey(string base64EncodedKey, bool isPrivateKey = false)
    {
        try
        {
            var keyBytes = Convert.FromBase64String(base64EncodedKey);
            using var rsa = RSA.Create();
            if (isPrivateKey)
            {
                rsa.ImportPkcs8PrivateKey(keyBytes, out _);
            }
            else
            {
                rsa.ImportRSAPublicKey(keyBytes, out _);
            }
                
            return new RsaSecurityKey(rsa.ExportParameters(isPrivateKey));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to import RSA key: {ex.Message}", ex);
        }
    }
}
