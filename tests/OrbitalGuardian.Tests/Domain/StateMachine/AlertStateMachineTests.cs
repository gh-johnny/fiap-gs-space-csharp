using FluentAssertions;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Tests.Domain.StateMachine;

public class AlertStateMachineTests
{
    private static Alert CreatePending() =>
        Alert.Create(Guid.NewGuid(), AlertSeverity.Warning, "Test alert");

    [Fact]
    public void PendingToAcknowledged_Succeeds()
    {
        var alert = CreatePending();
        alert.Acknowledge(DateTime.UtcNow);
        alert.Status.Should().Be(AlertStatus.Acknowledged);
        alert.AcknowledgedAt.Should().NotBeNull();
    }

    [Fact]
    public void AcknowledgedToPending_ThrowsInvalidStateTransitionException()
    {
        var alert = CreatePending();
        alert.Acknowledge(DateTime.UtcNow);
        var act = () => alert.Acknowledge(DateTime.UtcNow);
        act.Should().Throw<InvalidStateTransitionException>();
    }
}
