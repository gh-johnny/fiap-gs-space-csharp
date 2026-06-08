using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.EventHandlers;

public class AlertRaisedHandler : IDomainEventHandler<AlertRaisedEvent>
{
    public Task HandleAsync(AlertRaisedEvent domainEvent, CancellationToken ct)
    {
        OrbitalLogger.LogWarning("AlertRaised",
            $"Alert {domainEvent.AlertId} raised with severity {domainEvent.Severity} for conjunction {domainEvent.ConjunctionEventId}");
        return Task.CompletedTask;
    }
}
