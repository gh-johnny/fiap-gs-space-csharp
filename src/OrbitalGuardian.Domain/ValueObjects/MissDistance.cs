using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.ValueObjects;

public sealed class MissDistance : ValueObject
{
    public double ValueKm { get; private set; }
    public double TimeToClosestApproachSeconds { get; private set; }

    private MissDistance() { }

    /// <summary>Creates a MissDistance. Both valueKm and tcaSeconds must be >= 0.</summary>
    public static MissDistance Create(double valueKm, double tcaSeconds)
    {
        if (valueKm < 0)
            throw new InvalidStateVectorException("Miss distance cannot be negative.");
        if (tcaSeconds < 0)
            throw new InvalidStateVectorException("Time to closest approach cannot be negative.");

        return new MissDistance { ValueKm = valueKm, TimeToClosestApproachSeconds = tcaSeconds };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueKm;
        yield return TimeToClosestApproachSeconds;
    }
}
