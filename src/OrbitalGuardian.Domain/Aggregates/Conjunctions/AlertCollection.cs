using System.Collections;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Domain.Aggregates.Conjunctions;

public sealed class AlertCollection : IEnumerable<Alert>
{
    private readonly IReadOnlyList<Alert> _items;

    internal AlertCollection(IEnumerable<Alert> items) =>
        _items = items.ToList().AsReadOnly();

    public AlertCollection FilterUnacknowledged() =>
        new(_items.Where(a => a.Status == AlertStatus.Pending));

    public Alert? FindById(Guid id) =>
        _items.FirstOrDefault(a => a.Id == id);

    public IEnumerator<Alert> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
