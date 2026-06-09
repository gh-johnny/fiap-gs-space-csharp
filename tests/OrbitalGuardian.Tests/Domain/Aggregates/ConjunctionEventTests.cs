using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Aggregates;

public class ConjunctionEventTests
{
    private static ConjunctionEvent CreateEvent(double pc = 1e-3) =>
        ConjunctionEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddHours(2),
            MissDistance.Create(5.0, 7200),
            CollisionProbability.Create(pc));

    [Fact]
    public void Create_GeneratesOneAlert()
    {
        var ev = CreateEvent();
        ev.Alerts.Should().HaveCount(1);
    }

    [Fact]
    public void Create_RaisesConjunctionDetectedEvent()
    {
        var ev = CreateEvent();
        ev.GetDomainEvents().Should().ContainSingle(e => e is ConjunctionDetectedEvent);
    }

    [Fact]
    public void AcknowledgeAlert_SetsStatusAcknowledged()
    {
        var ev = CreateEvent();
        var alert = ev.Alerts.First();
        ev.AcknowledgeAlert(alert.Id, DateTime.UtcNow);
        ev.Alerts.FindById(alert.Id)!.Status.Should().Be(AlertStatus.Acknowledged);
    }

    [Fact]
    public void AcknowledgeAlert_Twice_ThrowsInvalidStateTransitionException()
    {
        var ev = CreateEvent();
        var alert = ev.Alerts.First();
        ev.AcknowledgeAlert(alert.Id, DateTime.UtcNow);
        var act = () => ev.AcknowledgeAlert(alert.Id, DateTime.UtcNow);
        act.Should().Throw<InvalidStateTransitionException>();
    }

    [Fact]
    public void Resolve_SetsStatusResolved()
    {
        var ev = CreateEvent();
        ev.Resolve();
        ev.Status.Should().Be(ConjunctionStatus.Resolved);
    }

    [Fact]
    public void Resolve_Twice_ThrowsInvalidStateTransitionException()
    {
        var ev = CreateEvent();
        ev.Resolve();
        var act = () => ev.Resolve();
        act.Should().Throw<InvalidStateTransitionException>();
    }
}
