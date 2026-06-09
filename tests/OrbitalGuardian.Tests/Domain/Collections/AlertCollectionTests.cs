using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Tests.Domain.Collections;

public class AlertCollectionTests
{
    private static ConjunctionEvent CreateEvent() =>
        ConjunctionEvent.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddHours(2),
            MissDistance.Create(5.0, 7200),
            CollisionProbability.Create(1e-3));

    [Fact]
    public void FilterUnacknowledged_BeforeAcknowledge_ReturnsAll()
    {
        var ev = CreateEvent();
        ev.Alerts.FilterUnacknowledged().Should().HaveCount(1);
    }

    [Fact]
    public void FilterUnacknowledged_AfterAcknowledge_ReturnsEmpty()
    {
        var ev = CreateEvent();
        var alertId = ev.Alerts.First().Id;
        ev.AcknowledgeAlert(alertId, DateTime.UtcNow);
        ev.Alerts.FilterUnacknowledged().Should().BeEmpty();
    }

    [Fact]
    public void FindById_Existing_ReturnsAlert()
    {
        var ev = CreateEvent();
        var alertId = ev.Alerts.First().Id;
        ev.Alerts.FindById(alertId).Should().NotBeNull();
    }

    [Fact]
    public void FindById_NonExisting_ReturnsNull()
    {
        var ev = CreateEvent();
        ev.Alerts.FindById(Guid.NewGuid()).Should().BeNull();
    }
}
