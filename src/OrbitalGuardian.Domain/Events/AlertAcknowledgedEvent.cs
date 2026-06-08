using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Events;

public record AlertAcknowledgedEvent(
    Guid AlertId,
    Guid ConjunctionEventId,
    DateTime AcknowledgedAt,
    DateTime OccurredAt) : IDomainEvent;
