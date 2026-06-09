using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Infrastructure.Persistence.Configurations;

namespace OrbitalGuardian.Infrastructure.Persistence;

public class OrbitalGuardianDbContext : DbContext
{
    public DbSet<SpaceObject> SpaceObjects => Set<SpaceObject>();
    public DbSet<TelemetryReading> TelemetryReadings => Set<TelemetryReading>();
    public DbSet<ConjunctionEvent> ConjunctionEvents => Set<ConjunctionEvent>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<User> Users => Set<User>();

    public OrbitalGuardianDbContext(DbContextOptions<OrbitalGuardianDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SpaceObjectConfiguration());
        modelBuilder.ApplyConfiguration(new TelemetryReadingConfiguration());
        modelBuilder.ApplyConfiguration(new ConjunctionEventConfiguration());
        modelBuilder.ApplyConfiguration(new AlertConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
