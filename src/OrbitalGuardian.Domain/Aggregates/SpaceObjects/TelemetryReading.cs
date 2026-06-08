using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public class TelemetryReading : Entity<Guid>
{
    public Guid SpaceObjectId { get; private set; }
    public StateVector StateVector { get; private set; } = null!;
    public DateTime Timestamp { get; private set; }

    private TelemetryReading() { }

    /// <summary>Creates a new telemetry reading. Rejects timestamps more than 1 day in the future.</summary>
    public static TelemetryReading Create(Guid spaceObjectId, StateVector stateVector, DateTime timestamp)
    {
        if (timestamp > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Telemetry timestamp cannot be more than 1 day in the future.");

        return new TelemetryReading
        {
            Id = Guid.NewGuid(),
            SpaceObjectId = spaceObjectId,
            StateVector = stateVector,
            Timestamp = timestamp
        };
    }
}
