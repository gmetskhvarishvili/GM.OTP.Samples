using GM.Messaging.Persistence.Inbox;
using GM.OTP.Sample.Domain.Events.Users;

namespace GM.OTP.Sample.Consumer.Worker.Handlers;

public sealed class UserConfirmationInitiatedHandler(IInboxProcessor inbox, ILogger<UserConfirmationInitiatedHandler> logger)
{
    public Task Handle(UserConfirmationInitiatedIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.OTP.Sample.Consumer.Worker] User confirmation initiated — Subject: {Subject}, ConfirmationType: {ConfirmationType}",
            message.Subject, message.ConfirmationType);

        // Ingest only: the InboxProcessorWorker poller runs GenerateOtp for this row.
        return inbox.IngestAsync(message, "GM.OTP.Sample.Consumer.Worker", ct);
    }
}