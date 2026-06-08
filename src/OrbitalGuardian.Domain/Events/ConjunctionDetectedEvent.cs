using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.Events;

public record ConjunctionDetectedEvent(
    Guid ConjunctionEventId,
    Guid PrimaryObjectId,
    Guid SecondaryObjectId,
    double CollisionProbabilityValue,
    DateTime OccurredAt) : IDomainEvent;
