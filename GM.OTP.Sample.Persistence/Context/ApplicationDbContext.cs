using System.Reflection;
using GM.EntityFramework.Domain.Common;
using GM.EntityFramework.Persistence;
using GM.EntityFramework.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GM.OTP.Sample.Persistence.Context;

// Not sealed: DesignTimeDbContextFactoryBase<TContext> constrains TContext to ApplicationDbContext,
// which requires an unsealed bound even though only this type is ever substituted.
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IClock? clock = null) : GenericDbContext(options)
{
    public const string DefaultSchema = "application";

    private readonly IClock _clock = clock ?? new SystemClock();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        AddAuditData();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAuditData();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddAuditData()
    {
        foreach (var entityEntry in ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Modified or EntityState.Added)
                     .ToList())
        {
            var entity = entityEntry.Entity;
            var utcNow = _clock.UtcNow;
            var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (entityEntry.State == EntityState.Added)
                SetPropertyIfExists(properties, entity, "CreatedAt", utcNow);
            SetPropertyIfExists(properties, entity, "UpdatedAt", utcNow);
        }
    }

    private static void SetPropertyIfExists(PropertyInfo[] props, object entity, string name, object? value)
    {
        props.FirstOrDefault(p => p.Name == name && p.CanWrite)?.SetValue(entity, value);
    }
}
