using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.EventHandlers;

public class AlertAcknowledgedHandler : IDomainEventHandler<AlertAcknowledgedEvent>
{
    public Task HandleAsync(AlertAcknowledgedEvent domainEvent, CancellationToken ct)
    {
        OrbitalLogger.LogInfo("AlertAcknowledged",
            $"Alert {domainEvent.AlertId} acknowledged at {domainEvent.AcknowledgedAt:u}");
        return Task.CompletedTask;
    }
}
