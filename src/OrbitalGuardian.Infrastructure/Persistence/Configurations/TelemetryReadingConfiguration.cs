using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;

namespace OrbitalGuardian.Infrastructure.Persistence.Configurations;

public class TelemetryReadingConfiguration : IEntityTypeConfiguration<TelemetryReading>
{
    public void Configure(EntityTypeBuilder<TelemetryReading> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.SpaceObjectId);
        builder.Property(x => x.Timestamp);

        builder.OwnsOne(x => x.StateVector, sv =>
        {
            sv.Property(p => p.PositionalUncertaintyKm).HasColumnName("SV_UncertaintyKm");
            sv.OwnsOne(p => p.Position, pos =>
            {
                pos.Property(c => c.X).HasColumnName("SV_Pos_X");
                pos.Property(c => c.Y).HasColumnName("SV_Pos_Y");
                pos.Property(c => c.Z).HasColumnName("SV_Pos_Z");
            });
            sv.OwnsOne(p => p.Velocity, vel =>
            {
                vel.Property(c => c.X).HasColumnName("SV_Vel_X");
                vel.Property(c => c.Y).HasColumnName("SV_Vel_Y");
                vel.Property(c => c.Z).HasColumnName("SV_Vel_Z");
            });
        });
    }
}
