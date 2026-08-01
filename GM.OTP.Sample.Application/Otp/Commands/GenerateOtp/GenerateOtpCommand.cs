using FluentValidation;
using GM.Mediator.Contracts;
using GM.OTP.Models;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate;
using GM.OTP.Sample.Domain.Events.Otp;
using GM.OTP.Sample.Domain.SeedWork;
using GM.OTP.Services;

namespace GM.OTP.Sample.Application.Otp.Commands.GenerateOtp;

public class GenerateOtpCommand : IRequest<GenerateOtpResult>
{
    public string? Subject { get; set; }
    public string? Destination { get; set; }
    public string? Purpose { get; set; }
    public int Channel { get; set; }
    public Guid? UserId { get; set; }
}

public class GenerateOtpCommandValidator : AbstractValidator<GenerateOtpCommand>
{
    public GenerateOtpCommandValidator()
    {
        RuleFor(x => x.Subject).NotNull().NotEmpty();
        RuleFor(x => x.Destination).NotNull().NotEmpty();
        RuleFor(x => x.Purpose).NotNull().NotEmpty();
    }
}

public class GenerateOtpCommandHandler(
    OtpManager otpManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateOtpCommand, GenerateOtpResult>
{
    public async Task<GenerateOtpResult> Handle(
        GenerateOtpCommand request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;

        var existing = await unitOfWork.OtpChallengeRepository
            .FindAsync(x =>
                    x.Subject == request.Subject
                    && x.Purpose == request.Purpose
                    && !x.IsUsed
                    && !x.IsInvalidated,
                false, null, cancellationToken);

        foreach (var c in existing)
        {
            c.Invalidate(nowUtc);
            unitOfWork.OtpChallengeRepository.Update(c);
        }
        
        var data = otpManager.Generate(request.Subject!, request.Destination!);

        var challenge = OtpChallenge.Create(
            request.Subject!,
            request.Destination!,
            data.CodeHash,
            data.Salt,
            request.Purpose!,
            data.ExpiresAtUtc,
            data.MaxAttempts,
            request.UserId);

        await unitOfWork.OtpChallengeRepository.AddAsync(challenge, cancellationToken);
        
        var text = data.PlainCode;

        var generated = new OtpGeneratedIntegrationEvent(
            request.Destination!,
            request.Channel,
            request.Purpose!,
            text)
        {
            UserId = request.UserId
        };

        await unitOfWork.OutboxMessageRepository.AddAsync(
            OutboxMessage.From(request.UserId, generated),
            cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new GenerateOtpResult(
            challenge.Id,
            challenge.ExpiresAtUtc);
    }
}
