using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate;
using Xunit;

namespace GM.OTP.Sample.Tests;

public class OtpChallengeTests
{
    [Fact]
    public void Create_delegates_to_the_base_and_produces_an_active_challenge()
    {
        var now = DateTime.UtcNow;

        var challenge = OtpChallenge.Create(
            "subject", "destination", "hash", "salt", "login", now.AddMinutes(5), maxAttempts: 3);

        Assert.Equal("subject", challenge.Subject);
        Assert.True(challenge.IsActive(now));
        Assert.False(challenge.IsExpired(now));
    }
}
