using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Events;

public record AlertRaisedEvent(
    Guid AlertId,
    Guid ConjunctionEventId,
    AlertSeverity Severity,
    DateTime OccurredAt) : IDomainEvent;
