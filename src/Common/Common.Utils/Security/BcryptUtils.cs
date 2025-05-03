
namespace Common.Utils.Security;

public static class BcryptUtils
{
    public static string HashPassword(string password, string pepper)
    {
        var pepperedPassword = password + pepper;
        
        return BCrypt.Net.BCrypt.HashPassword(pepperedPassword);
    }
    
    public static bool VerifyPassword(string password, string hash, string pepper)
    {
        var pepperedPassword = password + pepper;
        
        return BCrypt.Net.BCrypt.Verify(pepperedPassword, hash);
    }
    
}
