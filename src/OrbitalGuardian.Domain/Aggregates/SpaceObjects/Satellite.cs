using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Aggregates.SpaceObjects;

public sealed class Satellite : SpaceObject
{
    public string Operator { get; private set; } = null!;
    public string MissionType { get; private set; } = null!;
    public double MassKg { get; private set; }

    private Satellite() { }

    private Satellite(
        Guid id,
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        string @operator,
        string missionType,
        double massKg)
        : base(id, name, noradId, SpaceObjectType.Satellite, launchDate, orbitalElements)
    {
        Operator = @operator;
        MissionType = missionType;
        MassKg = massKg;
    }

    /// <summary>Creates a new Satellite aggregate.</summary>
    public static Satellite Create(
        string name,
        string noradId,
        DateTime launchDate,
        OrbitalElements orbitalElements,
        string @operator,
        string missionType,
        double massKg)
        => new(Guid.NewGuid(), name, noradId, launchDate, orbitalElements, @operator, missionType, massKg);
}
