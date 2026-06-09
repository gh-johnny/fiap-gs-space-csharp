using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;

namespace OrbitalGuardian.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ConjunctionEventId);
        builder.Property(x => x.Severity).HasConversion<int>();
        builder.Property(x => x.Message).IsRequired().HasMaxLength(500);
        builder.Property(x => x.IssuedAt);
        builder.Property(x => x.AcknowledgedAt);
        builder.Property(x => x.Status).HasConversion<int>();
    }
}
