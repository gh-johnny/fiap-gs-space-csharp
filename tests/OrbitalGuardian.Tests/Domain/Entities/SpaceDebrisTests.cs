using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Entities;

public class SpaceDebrisTests
{
    [Fact]
    public void Create_SetsSpecificFields()
    {
        var oe = OrbitalElements.Create(82.9, 0.005, 14.7, 305, 140, 220);
        var debris = SpaceDebris.Create("SL-8 DEB", "14427", DateTime.UtcNow.AddYears(-20), oe, "Cosmos 3M", 0.5);
        debris.Name.Should().Be("SL-8 DEB");
        debris.NoradId.Should().Be("14427");
        debris.OriginObject.Should().Be("Cosmos 3M");
        debris.EstimatedSizeM.Should().Be(0.5);
        debris.IsActive.Should().BeTrue();
    }
}
