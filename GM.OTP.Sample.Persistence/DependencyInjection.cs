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
        services.AddEntityFrameworkNpgsql();

        services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                }).UseSnakeCaseNamingConvention();
            options.UseInternalServiceProvider(serviceProvider);
        });

        services.AddTransient<IOtpChallengeRepository, OtpChallengeRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Every host can inspect the outbox and requeue dead letters (the API maps it under /admin/messaging).
        services.AddGMOutboxAdministration<OutboxMessage, ApplicationDbContext>();

        // Only the process that relays the outbox (the producer worker) passes true; the API and the
        // background worker must not, or several pollers would read the same outbox table and publish twice.
        if (runOutboxRelay)
            services.AddGMOutboxRelay<OutboxMessage, ApplicationDbContext>();

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
                }).UseSnakeCaseNamingConvention();
        });

        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
        // UnitOfWork's constructor needs all three repositories, even though this worker only writes
        // to the inbox — register them so IUnitOfWork can be resolved.
        services.AddTransient<IOtpChallengeRepository, OtpChallengeRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddGMInbox<InboxMessage, ApplicationDbContext>();
        
        return services;
    }
}