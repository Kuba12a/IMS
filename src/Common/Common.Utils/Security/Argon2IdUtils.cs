using System.Security.Cryptography;

namespace Common.Utils.Security;

using Isopoh.Cryptography.Argon2;

public static class Argon2IdUtils
{
    public static string HashPassword(string password, string pepper)
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var salt = Convert.ToBase64String(saltBytes);

        var saltedPepperedPassword = password + salt + pepper;
        return Argon2.Hash(saltedPepperedPassword);
    }
    
    public static bool VerifyPassword(string password, string hash, string pepper)
    {
        var pepperedPassword = password + pepper;
        
        var isValid = Argon2.Verify(hash, System.Text.Encoding.UTF8.GetBytes(pepperedPassword));
        return isValid;
    }
}
