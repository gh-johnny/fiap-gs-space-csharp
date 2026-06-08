using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.EventHandlers;

public class ConjunctionDetectedHandler : IDomainEventHandler<ConjunctionDetectedEvent>
{
    public Task HandleAsync(ConjunctionDetectedEvent domainEvent, CancellationToken ct)
    {
        OrbitalLogger.LogInfo("ConjunctionDetected",
            $"Conjunction {domainEvent.ConjunctionEventId} detected between {domainEvent.PrimaryObjectId} and {domainEvent.SecondaryObjectId}. Pc={domainEvent.CollisionProbabilityValue:E2}");
        return Task.CompletedTask;
    }
}
