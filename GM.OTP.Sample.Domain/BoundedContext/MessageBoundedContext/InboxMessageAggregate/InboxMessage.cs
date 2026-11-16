using GM.EntityFramework.Domain.Abstractions;

namespace GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;

/// <summary>
/// The service's inbox row. GM.Messaging creates it (InboxMessage.Create&lt;T&gt;, which only needs this
/// private constructor) and records attempts, failures and the inbox worker's state on it.
/// </summary>
public sealed class InboxMessage : GM.Messaging.Domain.Inbox.InboxMessage, IAggregateRoot
{
    private InboxMessage() { }
}
