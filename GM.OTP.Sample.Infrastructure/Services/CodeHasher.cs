using System.Security.Cryptography;
using System.Text;
using GM.OTP.Domain.Abstractions;

namespace GM.OTP.Sample.Infrastructure.Services;

public sealed class CodeHasher : ICodeHasher
{
    public string Hash(string value, string salt)
    {
        // HMAC-SHA256 keyed by the per-challenge salt over the composed value
        // (code + subject + destination). Recomputing with tampered inputs won't match.
        var key = Encoding.UTF8.GetBytes(salt);
        var bytes = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }

    public bool Verify(string hash, string value, string salt)
    {
        var computed = Convert.FromBase64String(Hash(value, salt));
        var stored = Convert.FromBase64String(hash);
        return CryptographicOperations.FixedTimeEquals(stored, computed);
    }
}
