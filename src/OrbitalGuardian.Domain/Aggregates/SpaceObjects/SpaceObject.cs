using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Events;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public abstract class SpaceObject : AggregateRoot<Guid>
{
    private readonly List<TelemetryReading> _telemetryReadings = new();

    public string Name { get; private set; } = null!;
    public string NoradId { get; private set; } = null!;
    public SpaceObjectType Type { get; private set; }
    public DateTime LaunchDate { get; private set; }
    public OrbitalElements OrbitalElements { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public TelemetryReadingCollection TelemetryReadings =>
        new TelemetryReadingCollection(_telemetryReadings);

    protected SpaceObject() { }

    protected SpaceObject(
        Guid id,
        string name,
        string noradId,
        SpaceObjectType type,
        DateTime launchDate,
        OrbitalElements orbitalElements) : base(id)
    {
        Name = name;
        NoradId = noradId;
        Type = type;
        LaunchDate = launchDate;
        OrbitalElements = orbitalElements;
        IsActive = true;
    }

    public void UpdateOrbitalElements(OrbitalElements newElements) =>
        OrbitalElements = newElements;

    public void RecordTelemetry(StateVector stateVector, DateTime timestamp)
    {
        var reading = TelemetryReading.Create(Id, stateVector, timestamp);
        _telemetryReadings.Add(reading);
        RaiseDomainEvent(new TelemetryRecordedEvent(Id, timestamp, DateTime.UtcNow));
    }

    public void Deactivate() => IsActive = false;
}
