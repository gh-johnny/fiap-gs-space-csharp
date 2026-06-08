using System.Collections;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public sealed class SpaceObjectCollection : IEnumerable<SpaceObject>
{
    private readonly IReadOnlyList<SpaceObject> _items;

    public SpaceObjectCollection(IEnumerable<SpaceObject> items) =>
        _items = items.ToList().AsReadOnly();

    public SpaceObjectCollection FilterActive() =>
        new(_items.Where(o => o.IsActive));

    public SpaceObject? FindByNoradId(string noradId) =>
        _items.FirstOrDefault(o => o.NoradId == noradId);

    public IEnumerator<SpaceObject> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
