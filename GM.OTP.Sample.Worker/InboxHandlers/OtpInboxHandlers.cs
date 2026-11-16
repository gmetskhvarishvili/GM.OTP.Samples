using GM.Mediator.Contracts;
using GM.Messaging.Persistence.Inbox;
using GM.OTP.Sample.Application.Otp.Commands.GenerateOtp;
using GM.OTP.Sample.Domain.Constants;
using GM.OTP.Sample.Domain.Events.Otp;
using GM.OTP.Sample.Domain.Events.Users;

namespace GM.OTP.Sample.Worker.InboxHandlers;

// The consumer worker ingests these events; GM.Messaging's inbox worker calls these handlers for
// them. Each runs in one transaction with marking the inbox row handled: the OTP challenge and the
// OtpGenerated outbox row GenerateOtpCommand saves commit together with it. A failure rolls back and
// is retried with backoff, then dead-lettered (Messaging:Inbox settings).

/// <summary>Sends the confirmation code when GM.Identity starts confirming a user's email or phone.</summary>
public sealed class UserConfirmationInitiatedInboxHandler(IMediator mediator)
    : IInboxHandler<UserConfirmationInitiatedIntegrationEvent>
{
    public Task HandleAsync(UserConfirmationInitiatedIntegrationEvent integrationEvent, CancellationToken cancellationToken) =>
        mediator.Send(new GenerateOtpCommand
        {
            Subject = integrationEvent.Subject,
            Destination = integrationEvent.Subject,
            Purpose = OtpPurpose.ConfirmUser,
            Channel = integrationEvent.ConfirmationType,
            UserId = integrationEvent.UserId
        }, cancellationToken);
}

/// <summary>Sends the second-factor code when GM.Identity issues a two-factor challenge.</summary>
public sealed class TwoFactorChallengeIssuedInboxHandler(IMediator mediator)
    : IInboxHandler<TwoFactorChallengeIssuedIntegrationEvent>
{
    // The code is stored under Subject (what GM.Identity verifies against) and sent to Destination over the
    // channel the user picked at login. The purpose must match what GM.Identity validates the second factor
    // against (grant_type=two_factor).
    public Task HandleAsync(TwoFactorChallengeIssuedIntegrationEvent integrationEvent, CancellationToken cancellationToken) =>
        mediator.Send(new GenerateOtpCommand
        {
            Subject = integrationEvent.Subject,
            Destination = integrationEvent.Destination ?? integrationEvent.Subject,
            Purpose = OtpPurpose.TwoFactor,
            Channel = integrationEvent.Channel,
            UserId = integrationEvent.UserId
        }, cancellationToken);
}

/// <summary>Generates a code another service asked for; the event is already framed in OTP terms.</summary>
public sealed class OtpRequestedInboxHandler(IMediator mediator) : IInboxHandler<OtpRequestedIntegrationEvent>
{
    public Task HandleAsync(OtpRequestedIntegrationEvent integrationEvent, CancellationToken cancellationToken) =>
        mediator.Send(new GenerateOtpCommand
        {
            Subject = integrationEvent.Subject,
            Destination = integrationEvent.Destination,
            Purpose = integrationEvent.Purpose,
            Channel = integrationEvent.Channel,
            UserId = integrationEvent.UserId
        }, cancellationToken);
}
