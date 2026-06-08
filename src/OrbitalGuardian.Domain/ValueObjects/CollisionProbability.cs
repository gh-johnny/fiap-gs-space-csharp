using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.ValueObjects;

public sealed class CollisionProbability : ValueObject
{
    private const double Threshold = 1e-4;

    public double Value { get; private set; }
    public bool ExceedsThreshold => Value >= Threshold;
    public RiskLevel RiskLevel => Value switch
    {
        < 1e-5  => Enums.RiskLevel.Low,
        < 1e-4  => Enums.RiskLevel.Medium,
        < 1e-3  => Enums.RiskLevel.High,
        _       => Enums.RiskLevel.Critical
    };

    private CollisionProbability() { }

    /// <summary>Creates a CollisionProbability. Value must be in [0, 1].</summary>
    public static CollisionProbability Create(double value)
    {
        if (value < 0 || value > 1)
            throw new InvalidCollisionProbabilityException($"Collision probability must be between 0 and 1, got {value}.");

        return new CollisionProbability { Value = value };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
