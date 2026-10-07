using System.Text.Json;
using GM.Mediator.Contracts;
using GM.OTP.Sample.Application.Otp.Commands.GenerateOtp;
using GM.OTP.Sample.Domain.Constants;
using GM.OTP.Sample.Domain.Events.Otp;
using GM.OTP.Sample.Domain.Events.Users;
using GM.OTP.Sample.Domain.SeedWork;

namespace GM.OTP.Sample.Worker.Workers;

/// <summary>
/// Polls unprocessed inbox messages and, for each one in its own DbContext scope, generates the OTP
/// challenge (reusing OtpManager), renders the notification text, and enqueues an
/// mark and the outbox write commit together. A per-message scope keeps one failure from polluting the
/// change tracker used for the next message.
/// </summary>
public sealed class InboxProcessorWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<InboxProcessorWorker> logger,
    IConfiguration configuration)
    : BackgroundService
{
    private const int DefaultBatchSize = 50;
    private const int DefaultMaxConcurrency = 10;

    private static readonly Dictionary<string, string> EventTypeMap = new()
    {
        { nameof(UserConfirmationInitiatedIntegrationEvent), typeof(UserConfirmationInitiatedIntegrationEvent).FullName! },
        { nameof(TwoFactorChallengeIssuedIntegrationEvent), typeof(TwoFactorChallengeIssuedIntegrationEvent).FullName! },
        { nameof(OtpRequestedIntegrationEvent), typeof(OtpRequestedIntegrationEvent).FullName! }
    };

    private readonly TimeSpan _pollInterval =
        TimeSpan.FromSeconds(configuration.GetValue("InboxProcessor:PollIntervalSeconds", 5));
    private readonly int _batchSize =
        configuration.GetValue("InboxProcessor:BatchSize", DefaultBatchSize);
    private readonly int _maxConcurrency =
        configuration.GetValue("InboxProcessor:MaxConcurrency", DefaultMaxConcurrency);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Inbox processing batch failed.");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        // Read the pending messages in a short-lived scope; process each in its own scope below.
        List<(Guid EventId, string EventType, string Payload)> pendingMessages;
        using (var scope = scopeFactory.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var pending = await unitOfWork.InboxMessageRepository.FindAsync(
                x => x.ProcessedAtUtc == null && x.Error == null,
                true,
                null,
                cancellationToken);

            pendingMessages = pending
                .Take(_batchSize)
                .Select(x => (x.EventId, x.EventType, x.Payload))
                .ToList();
        }

        if (pendingMessages.Count == 0)
            return;

        logger.LogInformation("Processing {Count} inbox messages.", pendingMessages.Count);

        // Process messages in parallel with concurrency limit
        using var semaphore = new SemaphoreSlim(_maxConcurrency, _maxConcurrency);
        var tasks = pendingMessages.Select(msg => ProcessOneWithSemaphoreAsync(msg, semaphore, cancellationToken)).ToList();

        await Task.WhenAll(tasks);
    }

    private async Task ProcessOneWithSemaphoreAsync(
        (Guid EventId, string EventType, string Payload) message,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            await ProcessOneAsync(message.EventId, message.EventType, message.Payload, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessOneAsync(Guid eventId, string eventType, string payload, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Re-check to ensure message hasn't been processed by another worker
        var inbox = await unitOfWork.InboxMessageRepository.FirstOrDefaultAsync(
            x => x.EventId == eventId && x.ProcessedAtUtc == null && x.Error == null,
            false,
            null,
            cancellationToken);

        if (inbox is null)
            return;

        try
        {
            await HandleEventAsync(eventType, payload, mediator, cancellationToken);

            inbox.MarkProcessed();
            unitOfWork.InboxMessageRepository.Update(inbox);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully processed inbox message {EventId}.", eventId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process inbox event {EventId}.", eventId);
            await MarkFailedAsync(eventId, ex.Message, cancellationToken);
        }
    }

    private async Task MarkFailedAsync(Guid eventId, string error, CancellationToken cancellationToken)
    {
        // Fresh scope: the scope that just failed may have a polluted change tracker, so mark the
        // inbox row failed through a clean DbContext.
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var inbox = await unitOfWork.InboxMessageRepository.FirstOrDefaultAsync(
            x => x.EventId == eventId,
            false,
            null,
            cancellationToken);

        if (inbox is null)
            return;

        inbox.MarkFailed(error);
        unitOfWork.InboxMessageRepository.Update(inbox);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogWarning("Marked inbox message {EventId} as failed: {Error}", eventId, error);
    }

    private static async Task HandleEventAsync(string eventType, string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var typeFullName = eventType.Contains('.') ? eventType :
            EventTypeMap.Values.FirstOrDefault(v => v.EndsWith('.' + eventType, StringComparison.Ordinal)) ?? eventType;

        switch (typeFullName)
        {
            case var t when t.EndsWith(nameof(UserConfirmationInitiatedIntegrationEvent), StringComparison.Ordinal):
                await HandleUserConfirmationInitiatedAsync(payload, mediator, cancellationToken);
                break;

            case var t when t.EndsWith(nameof(TwoFactorChallengeIssuedIntegrationEvent), StringComparison.Ordinal):
                await HandleTwoFactorChallengeIssuedAsync(payload, mediator, cancellationToken);
                break;

            case var t when t.EndsWith(nameof(OtpRequestedIntegrationEvent), StringComparison.Ordinal):
                await HandleOtpRequestedAsync(payload, mediator, cancellationToken);
                break;

            default:
                throw new InvalidOperationException($"Unknown event type: {eventType}");
        }
    }

    private static async Task HandleUserConfirmationInitiatedAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<UserConfirmationInitiatedIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        var command = new GenerateOtpCommand
        {
            Subject = evt.Subject,
            Destination = evt.Subject,
            Purpose = OtpPurpose.ConfirmUser,
            Channel = evt.ConfirmationType,
            UserId = evt.UserId
        };

        await mediator.Send(command, cancellationToken);
    }

    private static async Task HandleTwoFactorChallengeIssuedAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<TwoFactorChallengeIssuedIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        // The subject is the user's contact (email/phone); deliver over the default channel. The purpose
        // must match what GM.Identity validates the second factor against (grant_type=two_factor).
        var command = new GenerateOtpCommand
        {
            Subject = evt.Subject,
            Destination = evt.Subject,
            Purpose = OtpPurpose.TwoFactor,
            Channel = 0,
            UserId = evt.UserId
        };

        await mediator.Send(command, cancellationToken);
    }

    private static async Task HandleOtpRequestedAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<OtpRequestedIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        // OtpRequested is already framed in OTP terms — pass its fields straight through.
        var command = new GenerateOtpCommand
        {
            Subject = evt.Subject,
            Destination = evt.Destination,
            Purpose = evt.Purpose,
            Channel = evt.Channel,
            UserId = evt.UserId
        };

        await mediator.Send(command, cancellationToken);
    }
}
