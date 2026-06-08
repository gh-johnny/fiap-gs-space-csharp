using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.Conjunctions;

public class ConjunctionEvent : AggregateRoot<Guid>, IStateMachine<ConjunctionStatus>, IBuildable
{
    private readonly List<Alert> _alerts = new();

    public Guid PrimaryObjectId { get; private set; }
    public Guid SecondaryObjectId { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public DateTime PredictedTcaUtc { get; private set; }
    public MissDistance MissDistance { get; private set; } = null!;
    public CollisionProbability CollisionProbability { get; private set; } = null!;
    public ConjunctionStatus Status { get; private set; }

    public AlertCollection Alerts => new(_alerts);

    private ConjunctionEvent() { }

    private ConjunctionEvent(
        Guid id,
        Guid primaryObjectId,
        Guid secondaryObjectId,
        DateTime predictedTcaUtc,
        MissDistance missDistance,
        CollisionProbability collisionProbability) : base(id)
    {
        PrimaryObjectId = primaryObjectId;
        SecondaryObjectId = secondaryObjectId;
        DetectedAt = DateTime.UtcNow;
        PredictedTcaUtc = predictedTcaUtc;
        MissDistance = missDistance;
        CollisionProbability = collisionProbability;
        Status = ConjunctionStatus.Active;
        GenerateAlert();
    }

    public ConjunctionStatus CurrentState => Status;

    public bool CanTransitionTo(ConjunctionStatus state) =>
        Status == ConjunctionStatus.Active &&
        (state == ConjunctionStatus.Resolved || state == ConjunctionStatus.Expired);

    public void TransitionTo(ConjunctionStatus state)
    {
        if (!CanTransitionTo(state))
            throw new InvalidStateTransitionException(Status.ToString(), state.ToString());
        Status = state;
    }

    private void GenerateAlert()
    {
        var severity = CollisionProbability.RiskLevel switch
        {
            RiskLevel.Low      => AlertSeverity.Informational,
            RiskLevel.Medium   => AlertSeverity.Warning,
            RiskLevel.High     => AlertSeverity.Critical,
            RiskLevel.Critical => AlertSeverity.Emergency,
            _                  => AlertSeverity.Informational
        };

        var alert = Alert.Create(Id, severity, $"Conjunction detected with Pc={CollisionProbability.Value:E2}");
        _alerts.Add(alert);
        RaiseDomainEvent(new AlertRaisedEvent(alert.Id, Id, severity, DateTime.UtcNow));
    }

    /// <summary>Creates a new ConjunctionEvent. Raises ConjunctionDetectedEvent on creation.</summary>
    public static ConjunctionEvent Create(
        Guid primaryObjectId,
        Guid secondaryObjectId,
        DateTime predictedTcaUtc,
        MissDistance missDistance,
        CollisionProbability collisionProbability)
    {
        var conjunction = new ConjunctionEvent(
            Guid.NewGuid(),
            primaryObjectId,
            secondaryObjectId,
            predictedTcaUtc,
            missDistance,
            collisionProbability);

        conjunction.RaiseDomainEvent(new ConjunctionDetectedEvent(
            conjunction.Id,
            primaryObjectId,
            secondaryObjectId,
            collisionProbability.Value,
            DateTime.UtcNow));

        return conjunction;
    }

    public void Resolve() => TransitionTo(ConjunctionStatus.Resolved);

    public void AcknowledgeAlert(Guid alertId, DateTime acknowledgedAt)
    {
        var alert = Alerts.FindById(alertId)
            ?? throw new InvalidOperationException($"Alert {alertId} not found in conjunction {Id}.");
        alert.Acknowledge(acknowledgedAt);
        RaiseDomainEvent(new AlertAcknowledgedEvent(alertId, Id, acknowledgedAt, DateTime.UtcNow));
    }
}
