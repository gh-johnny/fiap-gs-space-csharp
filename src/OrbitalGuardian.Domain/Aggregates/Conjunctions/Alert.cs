using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Aggregates.Conjunctions;

public class Alert : Entity<Guid>, IStateMachine<AlertStatus>
{
    public Guid ConjunctionEventId { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string Message { get; private set; } = null!;
    public DateTime IssuedAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }
    public AlertStatus Status { get; private set; }

    private Alert() { }

    public static Alert Create(Guid conjunctionEventId, AlertSeverity severity, string message)
        => new()
        {
            Id = Guid.NewGuid(),
            ConjunctionEventId = conjunctionEventId,
            Severity = severity,
            Message = message,
            IssuedAt = DateTime.UtcNow,
            Status = AlertStatus.Pending
        };

    public AlertStatus CurrentState => Status;

    public bool CanTransitionTo(AlertStatus state) =>
        Status == AlertStatus.Pending && state == AlertStatus.Acknowledged;

    public void TransitionTo(AlertStatus state)
    {
        if (!CanTransitionTo(state))
            throw new InvalidStateTransitionException(Status.ToString(), state.ToString());
        Status = state;
    }

    public void Acknowledge(DateTime acknowledgedAt)
    {
        TransitionTo(AlertStatus.Acknowledged);
        AcknowledgedAt = acknowledgedAt;
    }
}
