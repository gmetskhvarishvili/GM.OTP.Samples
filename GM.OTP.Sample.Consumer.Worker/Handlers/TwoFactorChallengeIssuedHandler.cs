using GM.Messaging.Persistence.Inbox;
using GM.OTP.Sample.Domain.Events.Users;

namespace GM.OTP.Sample.Consumer.Worker.Handlers;

public sealed class TwoFactorChallengeIssuedHandler(IInboxProcessor inbox, ILogger<TwoFactorChallengeIssuedHandler> logger)
{
    public Task Handle(TwoFactorChallengeIssuedIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.OTP.Sample.Consumer.Worker] Two-factor challenge issued — Subject: {Subject}",
            message.Subject);

        // Ingest only: the InboxProcessorWorker poller runs GenerateOtp (purpose TwoFactor) for this row.
        return inbox.IngestAsync(message, "GM.OTP.Sample.Consumer.Worker", ct);
    }
}
