using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Collections;

public class SpaceObjectCollectionTests
{
    private static OrbitalElements Oe() =>
        OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);

    private static Satellite MakeSat(string norad, bool active = true)
    {
        var sat = Satellite.Create("SAT-" + norad, norad, DateTime.UtcNow.AddYears(-5), Oe(), "OP", "M", 500);
        if (!active) sat.Deactivate();
        return sat;
    }

    [Fact]
    public void FilterActive_ReturnsOnlyActive()
    {
        var col = new SpaceObjectCollection(new SpaceObject[] { MakeSat("1", true), MakeSat("2", false) });
        col.FilterActive().Should().HaveCount(1);
    }

    [Fact]
    public void FindByNoradId_Existing_ReturnsSatellite()
    {
        var col = new SpaceObjectCollection(new SpaceObject[] { MakeSat("25544") });
        col.FindByNoradId("25544").Should().NotBeNull();
    }

    [Fact]
    public void FindByNoradId_NonExisting_ReturnsNull()
    {
        var col = new SpaceObjectCollection(Array.Empty<SpaceObject>());
        col.FindByNoradId("99999").Should().BeNull();
    }
}
