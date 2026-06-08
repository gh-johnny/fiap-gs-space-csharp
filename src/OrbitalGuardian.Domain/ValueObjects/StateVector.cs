using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Domain.ValueObjects;

public sealed class StateVector : ValueObject
{
    public Coordinates Position { get; private set; } = null!;
    public Coordinates Velocity { get; private set; } = null!;
    public double PositionalUncertaintyKm { get; private set; }

    private StateVector() { }

    /// <summary>Creates a StateVector. Uncertainty must be >= 0.</summary>
    public static StateVector Create(Coordinates position, Coordinates velocity, double positionalUncertaintyKm)
    {
        if (positionalUncertaintyKm < 0)
            throw new InvalidStateVectorException("Positional uncertainty cannot be negative.");

        return new StateVector
        {
            Position = position,
            Velocity = velocity,
            PositionalUncertaintyKm = positionalUncertaintyKm
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Position;
        yield return Velocity;
        yield return PositionalUncertaintyKm;
    }
}
