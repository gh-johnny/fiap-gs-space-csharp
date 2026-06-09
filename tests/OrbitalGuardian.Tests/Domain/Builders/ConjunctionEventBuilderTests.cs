using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Builders;

public class ConjunctionEventBuilderTests
{
    [Fact]
    public void Build_WithoutRequiredFields_ThrowsInvalidOperationException()
    {
        var builder = new ConjunctionEventBuilder();
        var act = () => builder.Build();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Build_WithAllFields_ReturnsValidInstance()
    {
        var ev = new ConjunctionEventBuilder()
            .WithPrimaryObject(Guid.NewGuid())
            .WithSecondaryObject(Guid.NewGuid())
            .WithPredictedTca(DateTime.UtcNow.AddHours(3))
            .WithMissDistance(MissDistance.Create(10.0, 3600))
            .WithCollisionProbability(CollisionProbability.Create(1e-4))
            .Build();

        ev.Should().NotBeNull();
        ev.Alerts.Should().HaveCount(1);
    }
}
