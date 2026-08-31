using GM.OTP.Sample.Infrastructure;
using GM.OTP.Sample.Persistence;
using GM.OTP.Sample.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConsumerWorkerInfrastructure(
    builder.Configuration);
builder.Services.AddConsumerWorkerPersistence(builder.Configuration);

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
