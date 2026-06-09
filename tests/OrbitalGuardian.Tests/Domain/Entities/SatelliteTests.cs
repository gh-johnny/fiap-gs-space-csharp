using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Entities;

public class SatelliteTests
{
    private static OrbitalElements DefaultOe() =>
        OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);

    private static StateVector DefaultStateVector() =>
        StateVector.Create(
            Coordinates.Create(1000, 2000, 3000),
            Coordinates.Create(1, 2, 3),
            0.1);

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var sat = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), DefaultOe(), "NASA", "Research", 420_000);
        sat.Name.Should().Be("ISS");
        sat.NoradId.Should().Be("25544");
        sat.IsActive.Should().BeTrue();
        sat.Operator.Should().Be("NASA");
        sat.MassKg.Should().Be(420_000);
    }

    [Fact]
    public void RecordTelemetry_AddsReading()
    {
        var sat = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), DefaultOe(), "NASA", "Research", 420_000);
        var sv = DefaultStateVector();
        sat.RecordTelemetry(sv, DateTime.UtcNow.AddMinutes(-5));
        sat.TelemetryReadings.Should().HaveCount(1);
    }

    [Fact]
    public void TelemetryReadings_Latest_ReturnsNewest()
    {
        var sat = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), DefaultOe(), "NASA", "Research", 420_000);
        var older = DateTime.UtcNow.AddHours(-2);
        var newer = DateTime.UtcNow.AddHours(-1);
        sat.RecordTelemetry(DefaultStateVector(), older);
        sat.RecordTelemetry(DefaultStateVector(), newer);
        sat.TelemetryReadings.Latest()!.Timestamp.Should().Be(newer);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var sat = Satellite.Create("ISS", "25544", DateTime.UtcNow.AddYears(-10), DefaultOe(), "NASA", "Research", 420_000);
        sat.Deactivate();
        sat.IsActive.Should().BeFalse();
    }
}
