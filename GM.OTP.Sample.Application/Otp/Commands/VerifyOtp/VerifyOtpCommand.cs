using FluentValidation;
using GM.Mediator.Contracts;
using GM.OTP.Models;
using GM.OTP.Sample.Domain.SeedWork;
using GM.OTP.Services;

namespace GM.OTP.Sample.Application.Otp.Commands.VerifyOtp;

public class VerifyOtpCommand : IRequest<VerifyOtpResult>
{
    public string? Subject { get; set; }
    public string? Purpose { get; set; }
    public string? Code { get; set; }
}

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Subject).NotNull().NotEmpty();
        RuleFor(x => x.Purpose).NotNull().NotEmpty();
        RuleFor(x => x.Code).NotNull().NotEmpty();
    }
}

public class VerifyOtpCommandHandler(
    OtpManager otpManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyOtpCommand, VerifyOtpResult>
{
    public async Task<VerifyOtpResult> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var challenge = await unitOfWork.OtpChallengeRepository
            .FirstOrDefaultAsync(x => x.Subject == request.Subject
                                      && x.Purpose == request.Purpose
                                      && !x.IsUsed
                                      && !x.IsInvalidated
                                      && x.ExpiresAtUtc > nowUtc,
                false,
                null,
                cancellationToken);

        if (challenge is null)
            return new VerifyOtpResult(false, "Active OTP not found.");

        var result = otpManager.Verify(challenge, request.Code!);

        unitOfWork.OtpChallengeRepository.Update(challenge);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}