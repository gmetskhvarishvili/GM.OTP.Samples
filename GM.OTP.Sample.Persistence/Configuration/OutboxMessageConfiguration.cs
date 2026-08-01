using GM.Messaging.Persistence.Configuration;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate;

namespace GM.OTP.Sample.Persistence.Configuration;

public class OutboxMessageConfiguration() : OutboxMessageConfiguration<OutboxMessage>("outbox_messages");
