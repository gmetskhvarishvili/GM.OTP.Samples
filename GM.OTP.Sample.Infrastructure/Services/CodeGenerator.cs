using System.Security.Cryptography;
using GM.OTP.Domain.Abstractions;

namespace GM.OTP.Sample.Infrastructure.Services;

public class CodeGenerator : ICodeGenerator
{
    public string GenerateNumericCode(int length)
    {
        // Cryptographically secure and unbiased: draw each digit uniformly from 0-9.
        var digits = new char[length];
        for (var i = 0; i < length; i++)
            digits[i] = (char)('0' + RandomNumberGenerator.GetInt32(0, 10));

        return new string(digits);
    }

    public string GenerateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
