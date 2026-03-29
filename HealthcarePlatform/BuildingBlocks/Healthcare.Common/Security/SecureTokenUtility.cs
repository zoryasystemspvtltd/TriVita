using System.Security.Cryptography;
using System.Text;

namespace Healthcare.Common.Security;

public static class SecureTokenUtility
{
    public static string Sha256Hex(string value)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    public static string CreateUrlSafeToken(int byteLength = 48)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
