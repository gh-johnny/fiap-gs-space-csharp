using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.EventHandlers;

public class TelemetryRecordedHandler : IDomainEventHandler<TelemetryRecordedEvent>
{
    public Task HandleAsync(TelemetryRecordedEvent domainEvent, CancellationToken ct)
    {
        OrbitalLogger.LogDebug("TelemetryRecorded",
            $"Telemetry recorded for object {domainEvent.SpaceObjectId} at {domainEvent.Timestamp:u}");
        return Task.CompletedTask;
    }
}
