using GM.EntityFramework.Domain.Repositories;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate.Interfaces;

namespace GM.OTP.Sample.Domain.SeedWork;

public interface IUnitOfWork : IGenericUnitOfWork
{
    public IOtpChallengeRepository OtpChallengeRepository { get; }

    public IInboxMessageRepository InboxMessageRepository { get; }

    public IOutboxMessageRepository OutboxMessageRepository { get; }
}
