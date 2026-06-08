using System.Collections;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Domain.Aggregates.Conjunctions;

public sealed class ConjunctionEventCollection : IEnumerable<ConjunctionEvent>
{
    private readonly IReadOnlyList<ConjunctionEvent> _items;

    public ConjunctionEventCollection(IEnumerable<ConjunctionEvent> items) =>
        _items = items.ToList().AsReadOnly();

    public ConjunctionEventCollection FilterActive() =>
        new(_items.Where(c => c.Status == ConjunctionStatus.Active));

    public ConjunctionEvent? FindById(Guid id) =>
        _items.FirstOrDefault(c => c.Id == id);

    public IEnumerator<ConjunctionEvent> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
