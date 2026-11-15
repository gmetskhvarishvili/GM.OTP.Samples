using GM.EntityFramework.Persistence.Repositories;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Persistence.Context;

namespace GM.OTP.Sample.Persistence.Repositories;

public sealed class InboxMessageRepository(ApplicationDbContext context)
    : GenericRepository<InboxMessage, ApplicationDbContext>(context), IInboxMessageRepository;
