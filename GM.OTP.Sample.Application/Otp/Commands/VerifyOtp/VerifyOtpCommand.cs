using FluentValidation;
using GM.Mediator.Contracts;
using GM.OTP.Models;
using GM.OTP.Sample.Domain.SeedWork;
using GM.OTP.Services;

namespace GM.OTP.Sample.Application.Otp.Commands.VerifyOtp;

public sealed record VerifyOtpCommand : IRequest<VerifyOtpResult>
{
    public required string Subject { get; init; }
    public required string Purpose { get; init; }
    public required string Code { get; init; }
}

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Subject).NotNull().NotEmpty();
        RuleFor(x => x.Purpose).NotNull().NotEmpty();
        RuleFor(x => x.Code).NotNull().NotEmpty();
    }
}

public sealed class VerifyOtpCommandHandler(
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

        var result = otpManager.Verify(challenge, request.Code);

        unitOfWork.OtpChallengeRepository.Update(challenge);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}
