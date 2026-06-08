namespace OrbitalGuardian.Domain.Shared;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
