using System.Collections;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public sealed class TelemetryReadingCollection : IEnumerable<TelemetryReading>
{
    private readonly IReadOnlyList<TelemetryReading> _items;

    internal TelemetryReadingCollection(IEnumerable<TelemetryReading> items) =>
        _items = items.ToList().AsReadOnly();

    /// <summary>Returns the most recent telemetry reading by Timestamp, or null if empty.</summary>
    public TelemetryReading? Latest() =>
        _items.OrderByDescending(r => r.Timestamp).FirstOrDefault();

    public IEnumerator<TelemetryReading> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
