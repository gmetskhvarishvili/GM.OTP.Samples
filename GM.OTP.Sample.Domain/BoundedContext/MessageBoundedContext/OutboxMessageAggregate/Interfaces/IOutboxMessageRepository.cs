using GM.EntityFramework.Domain.Repositories;

namespace GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate.Interfaces;

public interface IOutboxMessageRepository : IGenericRepository<OutboxMessage>;
