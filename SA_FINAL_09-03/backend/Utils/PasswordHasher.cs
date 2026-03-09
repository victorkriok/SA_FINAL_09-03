using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string GerarHash(string senha, string salt)
    {
        using var sha256 = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(senha + salt);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}