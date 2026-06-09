using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;

namespace OrbitalGuardian.Infrastructure.Persistence.Configurations;

public class ConjunctionEventConfiguration : IEntityTypeConfiguration<ConjunctionEvent>
{
    public void Configure(EntityTypeBuilder<ConjunctionEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.PrimaryObjectId);
        builder.Property(x => x.SecondaryObjectId);
        builder.Property(x => x.DetectedAt);
        builder.Property(x => x.PredictedTcaUtc);
        builder.Property(x => x.Status).HasConversion<int>();

        builder.OwnsOne(x => x.MissDistance, md =>
        {
            md.Property(p => p.ValueKm).HasColumnName("MissDistanceKm");
            md.Property(p => p.TimeToClosestApproachSeconds).HasColumnName("TcaSeconds");
        });

        builder.OwnsOne(x => x.CollisionProbability, cp =>
        {
            cp.Property(p => p.Value).HasColumnName("CollisionProbabilityValue");
        });

        builder.HasMany(x => (IEnumerable<Alert>)x.Alerts)
               .WithOne()
               .HasForeignKey(a => a.ConjunctionEventId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
