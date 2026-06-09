using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrbitalGuardian.Infrastructure.Persistence;

public class OrbitalGuardianDbContextFactory : IDesignTimeDbContextFactory<OrbitalGuardianDbContext>
{
    public OrbitalGuardianDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrbitalGuardianDbContext>()
            .UseSqlite("Data Source=orbital-guardian-design.db")
            .Options;
        return new OrbitalGuardianDbContext(options);
    }
}
