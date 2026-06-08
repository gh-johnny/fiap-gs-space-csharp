using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.ValueObjects;

public sealed class Coordinates : ValueObject
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    private Coordinates() { }

    /// <summary>Creates a Coordinates value object. Rejects NaN and infinite values.</summary>
    public static Coordinates Create(double x, double y, double z)
    {
        if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z) ||
            double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z))
            throw new InvalidStateVectorException("Coordinates cannot be NaN or infinity.");

        return new Coordinates { X = x, Y = y, Z = z };
    }

    public double DistanceTo(Coordinates other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }
}
