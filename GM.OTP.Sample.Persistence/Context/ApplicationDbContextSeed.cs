using Microsoft.Extensions.Logging;

namespace GM.OTP.Sample.Persistence.Context;

public sealed class ApplicationDbContextSeed
{
    // Never instantiated: SeedAsync is static, but the type itself still needs to exist as the
    // ILogger<ApplicationDbContextSeed> category, so it can't be a static class (CS0718).
    private ApplicationDbContextSeed()
    {
    }

    /// <summary>
    /// Extension point for seed data. This sample has none, so it is a documented no-op rather
    /// than a placeholder try/catch/retry that could never actually run.
    /// </summary>
    public static Task SeedAsync(ApplicationDbContext context, ILogger<ApplicationDbContextSeed> logger) =>
        Task.CompletedTask;
}
