using GM.EntityFramework.Domain.Repositories;

namespace GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate.Interfaces;

// Uses the Sample.Domain OtpChallenge aggregate (derived from GM.OTP.Domain.Entities.OtpChallenge).
public interface IOtpChallengeRepository : IGenericRepository<OtpChallenge>;
