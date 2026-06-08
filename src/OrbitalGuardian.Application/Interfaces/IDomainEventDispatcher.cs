using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken ct);
}
