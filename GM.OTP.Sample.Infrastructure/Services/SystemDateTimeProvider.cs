using GM.OTP.Domain.Abstractions;

namespace GM.OTP.Sample.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
