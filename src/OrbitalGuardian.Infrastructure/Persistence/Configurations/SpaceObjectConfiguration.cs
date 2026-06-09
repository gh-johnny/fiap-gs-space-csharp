using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Infrastructure.Persistence.Configurations;

public class SpaceObjectConfiguration : IEntityTypeConfiguration<SpaceObject>
{
    public void Configure(EntityTypeBuilder<SpaceObject> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        // TPH discriminator
        builder.HasDiscriminator(x => x.Type)
            .HasValue<Satellite>(SpaceObjectType.Satellite)
            .HasValue<SpaceDebris>(SpaceObjectType.Debris)
            .HasValue<SpaceStation>(SpaceObjectType.SpaceStation);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NoradId).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.LaunchDate);
        builder.Property(x => x.IsActive);

        // OrbitalElements as owned type
        builder.OwnsOne(x => x.OrbitalElements, oe =>
        {
            oe.Property(p => p.Inclination).HasColumnName("OE_Inclination");
            oe.Property(p => p.Eccentricity).HasColumnName("OE_Eccentricity");
            oe.Property(p => p.MeanMotion).HasColumnName("OE_MeanMotion");
            oe.Property(p => p.RightAscension).HasColumnName("OE_RightAscension");
            oe.Property(p => p.ArgumentOfPerigee).HasColumnName("OE_ArgumentOfPerigee");
            oe.Property(p => p.MeanAnomaly).HasColumnName("OE_MeanAnomaly");
        });


        // Ignore computed collection property — EF Core uses the backing field directly
        builder.Ignore(x => x.TelemetryReadings);
        builder.HasMany<TelemetryReading>("_telemetryReadings")
               .WithOne()
               .HasForeignKey(t => t.SpaceObjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
