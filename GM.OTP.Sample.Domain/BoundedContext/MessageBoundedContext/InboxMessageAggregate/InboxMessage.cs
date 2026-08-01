using GM.EntityFramework.Domain.Abstractions;

namespace GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;

public class InboxMessage : GM.Messaging.Domain.Inbox.InboxMessage, IAggregateRoot
{
    private InboxMessage() { }

    public new static InboxMessage Create(Guid eventId, string consumerName, string eventType, string payload, Guid? userId = null) =>
        new()
        {
            EventId = eventId,
            ConsumerName = consumerName,
            EventType = eventType,
            Payload = payload,
            UserId = userId,
            ReceivedAtUtc = DateTime.UtcNow
        };
}