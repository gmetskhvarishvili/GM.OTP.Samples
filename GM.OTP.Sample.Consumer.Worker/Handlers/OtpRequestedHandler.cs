using GM.Messaging.Persistence.Inbox;
using GM.OTP.Sample.Domain.Events.Otp;

namespace GM.OTP.Sample.Consumer.Worker.Handlers;

public class OtpRequestedHandler(IInboxProcessor inbox, ILogger<OtpRequestedHandler> logger)
{
    public Task Handle(OtpRequestedIntegrationEvent message, CancellationToken ct)
    {
        logger.LogInformation(
            "[GM.OTP.Sample.Consumer.Worker] OTP requested — Subject: {Subject}, Destination: {Destination}, Purpose: {Purpose}, Channel: {Channel}",
            message.Subject, message.Destination, message.Purpose, message.Channel);

        // Ingest only: the InboxProcessorWorker poller runs GenerateOtp for this row.
        return inbox.IngestAsync(message, "GM.OTP.Sample.Consumer.Worker", ct);
    }
}
