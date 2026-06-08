using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public sealed class SpaceDebris : SpaceObject
{
    public string OriginObject { get; private set; } = null!;
    public double EstimatedSizeM { get; private set; }

    private SpaceDebris() { }

    private SpaceDebris(
        Guid id,
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        string originObject,
        double estimatedSizeM)
        : base(id, name, noradId, SpaceObjectType.Debris, launchDate, orbitalElements)
    {
        OriginObject = originObject;
        EstimatedSizeM = estimatedSizeM;
    }

    /// <summary>Creates a new SpaceDebris aggregate.</summary>
    public static SpaceDebris Create(
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        string originObject,
        double estimatedSizeM)
        => new(Guid.NewGuid(), name, noradId, launchDate, orbitalElements, originObject, estimatedSizeM);
}
