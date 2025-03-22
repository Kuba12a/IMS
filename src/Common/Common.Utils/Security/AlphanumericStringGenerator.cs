using System.Security.Cryptography;
using System.Text;

namespace Common.Utils.Security;

public static class AlphanumericRandomStringGenerator
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateAlphanumericToken(int length = 32)
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomBytes = new byte[length];
            
        randomNumberGenerator.GetBytes(randomBytes);
            
        var result = new StringBuilder(length);
            
        foreach (byte b in randomBytes)
        {
            result.Append(AllowedChars[b % AllowedChars.Length]);
        }
            
        return result.ToString();
    }
}
