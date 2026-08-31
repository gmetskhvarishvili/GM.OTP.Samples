using GM.EntityFramework.Persistence.Repositories;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate.Interfaces;
using GM.OTP.Sample.Persistence.Context;

namespace GM.OTP.Sample.Persistence.Repositories;

public sealed class OtpChallengeRepository(ApplicationDbContext context)
    : GenericRepository<OtpChallenge,
        ApplicationDbContext>(context), IOtpChallengeRepository;

