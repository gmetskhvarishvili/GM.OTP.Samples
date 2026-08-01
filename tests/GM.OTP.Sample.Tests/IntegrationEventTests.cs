using GM.OTP.Sample.Domain.Events.Otp;
using Xunit;

namespace GM.OTP.Sample.Tests;

public class IntegrationEventTests
{
    [Fact]
    public void OtpGenerated_gets_an_id_and_carries_its_payload()
    {
        var evt = new OtpGeneratedIntegrationEvent("user@x.com", 2, "login", "Your code is 123456")
        {
            UserId = Guid.NewGuid()
        };

        Assert.NotEqual(Guid.Empty, evt.EventId);
        Assert.NotEqual(default, evt.OccurredAtUtc);
        Assert.Equal("user@x.com", evt.Destination);
        Assert.Equal(2, evt.Channel);
        Assert.NotNull(evt.UserId);
    }

    [Fact]
    public void OtpRequested_carries_its_payload()
    {
        var evt = new OtpRequestedIntegrationEvent("user-1", "user@x.com", "login", 1);

        Assert.NotEqual(Guid.Empty, evt.EventId);
        Assert.Equal("user-1", evt.Subject);
        Assert.Equal("user@x.com", evt.Destination);
        Assert.Equal(1, evt.Channel);
    }
}
