using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.StateMachine;

public class ConjunctionEventStateMachineTests
{
    private static ConjunctionEvent CreateActive() =>
        ConjunctionEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddHours(2),
            MissDistance.Create(5.0, 7200),
            CollisionProbability.Create(1e-3));

    [Fact]
    public void ActiveToResolved_Succeeds()
    {
        var ev = CreateActive();
        ev.Resolve();
        ev.Status.Should().Be(OrbitalGuardian.Domain.Enums.ConjunctionStatus.Resolved);
    }

    [Fact]
    public void ActiveToExpired_Succeeds()
    {
        var ev = CreateActive();
        ev.TransitionTo(OrbitalGuardian.Domain.Enums.ConjunctionStatus.Expired);
        ev.Status.Should().Be(OrbitalGuardian.Domain.Enums.ConjunctionStatus.Expired);
    }

    [Fact]
    public void ResolvedToActive_ThrowsInvalidStateTransitionException()
    {
        var ev = CreateActive();
        ev.Resolve();
        var act = () => ev.TransitionTo(OrbitalGuardian.Domain.Enums.ConjunctionStatus.Active);
        act.Should().Throw<InvalidStateTransitionException>();
    }

    [Fact]
    public void ExpiredToResolved_ThrowsInvalidStateTransitionException()
    {
        var ev = CreateActive();
        ev.TransitionTo(OrbitalGuardian.Domain.Enums.ConjunctionStatus.Expired);
        var act = () => ev.Resolve();
        act.Should().Throw<InvalidStateTransitionException>();
    }
}
