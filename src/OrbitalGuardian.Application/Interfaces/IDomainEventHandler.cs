using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Interfaces;

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct);
}
