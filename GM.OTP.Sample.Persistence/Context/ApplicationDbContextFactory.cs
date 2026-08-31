using GM.OTP.Sample.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GM.OTP.Sample.Persistence.Context;

public sealed class ApplicationDbContextFactory : DesignTimeDbContextFactoryBase<ApplicationDbContext>
{
    protected override ApplicationDbContext CreateNewInstance(DbContextOptions<ApplicationDbContext> options)
    {
        return new ApplicationDbContext(options);
    }
}