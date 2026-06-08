using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.ValueObjects;

public sealed class OrbitalElements : ValueObject
{
    public double Inclination { get; private set; }
    public double Eccentricity { get; private set; }
    public double MeanMotion { get; private set; }
    public double RightAscension { get; private set; }
    public double ArgumentOfPerigee { get; private set; }
    public double MeanAnomaly { get; private set; }

    private OrbitalElements() { }

    /// <summary>Creates and validates a new OrbitalElements value object.</summary>
    public static OrbitalElements Create(
        double inclination,
        double eccentricity,
        double meanMotion,
        double rightAscension,
        double argumentOfPerigee,
        double meanAnomaly)
    {
        if (inclination < 0 || inclination > 180)
            throw new InvalidOrbitalElementsException($"Inclination must be between 0 and 180 degrees, got {inclination}.");
        if (eccentricity < 0 || eccentricity > 1)
            throw new InvalidOrbitalElementsException($"Eccentricity must be between 0 and 1, got {eccentricity}.");
        if (meanMotion <= 0)
            throw new InvalidOrbitalElementsException($"Mean motion must be positive, got {meanMotion}.");

        return new OrbitalElements
        {
            Inclination = inclination,
            Eccentricity = eccentricity,
            MeanMotion = meanMotion,
            RightAscension = rightAscension,
            ArgumentOfPerigee = argumentOfPerigee,
            MeanAnomaly = meanAnomaly,
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Inclination;
        yield return Eccentricity;
        yield return MeanMotion;
        yield return RightAscension;
        yield return ArgumentOfPerigee;
        yield return MeanAnomaly;
    }
}
