using GM.Messaging.Persistence.Configuration;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;

namespace GM.OTP.Sample.Persistence.Configuration;

public class InboxMessageConfiguration() : InboxMessageConfiguration<InboxMessage>("inbox_messages");
