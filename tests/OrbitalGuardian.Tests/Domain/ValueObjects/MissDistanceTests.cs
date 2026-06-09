using FluentAssertions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.ValueObjects;

public class MissDistanceTests
{
    [Fact]
    public void Create_ValidValues_ReturnsInstance()
    {
        var md = MissDistance.Create(10.5, 120.0);
        md.ValueKm.Should().Be(10.5);
        md.TimeToClosestApproachSeconds.Should().Be(120.0);
    }

    [Fact]
    public void Create_NegativeDistance_ThrowsException()
    {
        var act = () => MissDistance.Create(-1, 0);
        act.Should().Throw<InvalidStateVectorException>();
    }

    [Fact]
    public void Create_NegativeTca_ThrowsException()
    {
        var act = () => MissDistance.Create(0, -1);
        act.Should().Throw<InvalidStateVectorException>();
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = MissDistance.Create(5.0, 60.0);
        var b = MissDistance.Create(5.0, 60.0);
        a.Should().Be(b);
    }
}
