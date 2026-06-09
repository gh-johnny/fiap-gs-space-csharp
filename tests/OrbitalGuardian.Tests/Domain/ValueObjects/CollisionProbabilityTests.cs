using FluentAssertions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.ValueObjects;

public class CollisionProbabilityTests
{
    [Fact]
    public void Create_ZeroValue_IsLow()
    {
        var cp = CollisionProbability.Create(0.0);
        cp.RiskLevel.Should().Be(RiskLevel.Low);
        cp.ExceedsThreshold.Should().BeFalse();
    }

    [Fact]
    public void Create_5e4_IsHigh()
    {
        var cp = CollisionProbability.Create(5e-4);
        cp.RiskLevel.Should().Be(RiskLevel.High);
        cp.ExceedsThreshold.Should().BeTrue();
    }

    [Fact]
    public void Create_1e3_IsCritical()
    {
        var cp = CollisionProbability.Create(1e-3);
        cp.RiskLevel.Should().Be(RiskLevel.Critical);
    }

    [Fact]
    public void Create_Above1_ThrowsException()
    {
        var act = () => CollisionProbability.Create(1.1);
        act.Should().Throw<InvalidCollisionProbabilityException>();
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = CollisionProbability.Create(0.001);
        var b = CollisionProbability.Create(0.001);
        a.Should().Be(b);
    }
}
