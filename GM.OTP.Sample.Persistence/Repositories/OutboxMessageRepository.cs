using GM.EntityFramework.Persistence.Repositories;
using GM.Messaging.Persistence.Outbox;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Persistence.Context;

namespace GM.OTP.Sample.Persistence.Repositories;

public sealed class OutboxMessageRepository(ApplicationDbContext context)
    : GenericRepository<OutboxMessage, ApplicationDbContext>(context), IOutboxMessageRepository, IOutboxDbContext<OutboxMessage>
{
    IQueryable<OutboxMessage> IOutboxDbContext<OutboxMessage>.OutboxMessages => _context.Set<OutboxMessage>();

    Task<int> IOutboxDbContext<OutboxMessage>.SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}
