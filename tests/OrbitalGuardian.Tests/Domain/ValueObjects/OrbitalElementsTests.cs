using FluentAssertions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.ValueObjects;

public class OrbitalElementsTests
{
    [Fact]
    public void Create_WithValidValues_ReturnsInstance()
    {
        var oe = OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);
        oe.Inclination.Should().Be(51.6);
        oe.Eccentricity.Should().Be(0.0001);
        oe.MeanMotion.Should().Be(15.5);
    }

    [Fact]
    public void Create_WithInclinationAbove180_ThrowsException()
    {
        var act = () => OrbitalElements.Create(181, 0, 15, 0, 0, 0);
        act.Should().Throw<InvalidOrbitalElementsException>();
    }

    [Fact]
    public void Create_WithNegativeInclination_ThrowsException()
    {
        var act = () => OrbitalElements.Create(-1, 0, 15, 0, 0, 0);
        act.Should().Throw<InvalidOrbitalElementsException>();
    }

    [Fact]
    public void Create_WithEccentricityAbove1_ThrowsException()
    {
        var act = () => OrbitalElements.Create(45, 1.1, 15, 0, 0, 0);
        act.Should().Throw<InvalidOrbitalElementsException>();
    }

    [Fact]
    public void Equality_WithSameValues_AreEqual()
    {
        var a = OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);
        var b = OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300);
        a.Should().Be(b);
    }
}
