using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public sealed class SpaceStation : SpaceObject
{
    public int CrewCapacity { get; private set; }
    public string Agency { get; private set; } = null!;

    private SpaceStation() { }

    private SpaceStation(
        Guid id,
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        int crewCapacity,
        string agency)
        : base(id, name, noradId, SpaceObjectType.SpaceStation, launchDate, orbitalElements)
    {
        CrewCapacity = crewCapacity;
        Agency = agency;
    }

    /// <summary>Creates a new SpaceStation aggregate.</summary>
    public static SpaceStation Create(
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        int crewCapacity,
        string agency)
        => new(Guid.NewGuid(), name, noradId, launchDate, orbitalElements, crewCapacity, agency);
}
