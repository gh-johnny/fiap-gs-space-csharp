using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Events;

public record TelemetryRecordedEvent(
    Guid SpaceObjectId,
    DateTime Timestamp,
    DateTime OccurredAt) : IDomainEvent;
