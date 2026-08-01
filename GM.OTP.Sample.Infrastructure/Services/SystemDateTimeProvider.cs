using GM.OTP.Domain.Abstractions;

namespace GM.OTP.Sample.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
