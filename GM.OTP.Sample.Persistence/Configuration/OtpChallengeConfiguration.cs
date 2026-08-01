using GM.OTP.Persistence.Configuration;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate;

namespace GM.OTP.Sample.Persistence.Configuration;

public class OtpChallengeConfiguration() : OtpChallengeConfiguration<OtpChallenge>("application", "OtpChallenges");