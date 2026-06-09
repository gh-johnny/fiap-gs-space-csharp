using FluentAssertions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.ValueObjects;

public class CoordinatesTests
{
    [Fact]
    public void DistanceTo_KnownVectors_IsCorrect()
    {
        var a = Coordinates.Create(0, 0, 0);
        var b = Coordinates.Create(3, 4, 0);
        a.DistanceTo(b).Should().BeApproximately(5.0, 1e-10);
    }

    [Fact]
    public void DistanceTo_3D_IsCorrect()
    {
        var a = Coordinates.Create(1, 2, 3);
        var b = Coordinates.Create(4, 6, 3);
        a.DistanceTo(b).Should().BeApproximately(5.0, 1e-10);
    }

    [Fact]
    public void Create_NaN_ThrowsException()
    {
        var act = () => Coordinates.Create(double.NaN, 0, 0);
        act.Should().Throw<InvalidStateVectorException>();
    }

    [Fact]
    public void Create_Infinity_ThrowsException()
    {
        var act = () => Coordinates.Create(0, double.PositiveInfinity, 0);
        act.Should().Throw<InvalidStateVectorException>();
    }
}
