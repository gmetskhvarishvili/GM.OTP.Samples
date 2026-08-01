using GM.API.Startup;
using GM.OTP;
using GM.OTP.Sample.Infrastructure;
using GM.OTP.Sample.Persistence;
using GM.OTP.Sample.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = ProgramExtension.CreateGMBuilder(args);

builder.Services.ConfigureGMServices(
    builder.Configuration,
    "policyName",
    "SwaggerDocOptions");

builder.Services.AddGMOtp();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseGMServices();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (context != null)
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContextSeed>>();
            if (logger != null)
                new ApplicationDbContextSeed().SeedAsync(context, logger).Wait();
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

app.Run();
