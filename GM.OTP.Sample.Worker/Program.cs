using GM.Messaging.Persistence;
using GM.OTP.Sample.Application;
using GM.OTP.Sample.Infrastructure;
using GM.OTP.Sample.Persistence;
using GM.OTP.Sample.Persistence.Context;
using GM.OTP.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.OTP.Sample.Domain.Events.Otp;
using GM.OTP.Sample.Domain.Events.Users;
using GM.OTP.Sample.Worker.InboxHandlers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddWorkerInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// GM.Messaging's inbox worker handles the events the consumer worker ingested, one handler per event
// type (settings: Messaging:Inbox).
builder.Services.AddGMInboxWorker<InboxMessage, ApplicationDbContext>();
builder.Services.AddGMInboxHandler<UserConfirmationInitiatedIntegrationEvent, UserConfirmationInitiatedInboxHandler>();
builder.Services.AddGMInboxHandler<TwoFactorChallengeIssuedIntegrationEvent, TwoFactorChallengeIssuedInboxHandler>();
builder.Services.AddGMInboxHandler<OtpRequestedIntegrationEvent, OtpRequestedInboxHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (context != null && (await context.Database.GetPendingMigrationsAsync()).Any())
            await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

await app.RunAsync();
