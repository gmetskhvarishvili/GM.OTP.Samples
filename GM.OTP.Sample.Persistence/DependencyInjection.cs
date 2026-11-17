using GM.Messaging;
using GM.Messaging.Persistence;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.OutboxMessageAggregate.Interfaces;
using GM.OTP.Sample.Domain.BoundedContext.OtpBoundedContext.OtpChallengeAggregate.Interfaces;
using GM.OTP.Sample.Domain.SeedWork;
using GM.OTP.Sample.Persistence.Context;
using GM.OTP.Sample.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GM.OTP.Sample.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, bool runOutboxRelay = false)
    {
        // Pooled contexts with only our own settings: AddGMOutbox (below) attaches GM.Messaging's save interceptors, among them
        // the one that wakes the relays as soon as a save adds outbox messages.
        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                }).UseSnakeCaseNamingConvention();
        });

        services.AddTransient<IOtpChallengeRepository, OtpChallengeRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Everything GM.Messaging needs, in one call: outbox administration (the API maps it under /admin/messaging),
        // erasure events, its save interceptors on ApplicationDbContext, the key ring for [SensitiveData] values
        // (Messaging:SensitiveData: OTP destinations and codes travel encrypted), and either the relay or the wake-up.
        // Only the producer worker passes runOutboxRelay: true and publishes the outbox; the API and the background
        // worker only save events, and wake the relay at once when they do.
        services.AddGMOutbox<OutboxMessage, ApplicationDbContext>(configuration, relay: runOutboxRelay);

        return services;
    }

    public static IServiceCollection AddConsumerWorkerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Only our own settings: AddGMInbox (below) attaches GM.Messaging's save interceptors to this DbContext.
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                }).UseSnakeCaseNamingConvention();
        });

        // The inbox, its save interceptors on ApplicationDbContext, and the key ring (Messaging:SensitiveData) that
        // decrypts [SensitiveData] values of incoming events. The handlers only ingest, so nothing else is registered.
        services.AddGMInbox<InboxMessage, ApplicationDbContext>(configuration);

        return services;
    }
}
