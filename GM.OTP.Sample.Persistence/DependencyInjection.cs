using GM.Messaging.Persistence;
using GM.Messaging.Persistence.Inbox;
using GM.Messaging.Persistence.Outbox;
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
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkNpgsql();

        services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                });
            options.UseInternalServiceProvider(serviceProvider);
        });

        services.AddTransient<IOtpChallengeRepository, OtpChallengeRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<IOutboxDbContext<OutboxMessage>, OutboxMessageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddConsumerWorkerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                });
        });

        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
        // UnitOfWork's constructor needs all three repositories, even though this worker only writes
        // to the inbox — register them so IUnitOfWork can be resolved.
        services.AddTransient<IOtpChallengeRepository, OtpChallengeRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<InboxMessageRepository>();
        services.AddTransient<IInboxStore<InboxMessage>, InboxMessageRepository>();
        services.AddGMInboxProcessor<InboxMessage>();
        
        return services;
    }
}