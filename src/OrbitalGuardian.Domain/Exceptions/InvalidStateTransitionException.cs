namespace OrbitalGuardian.Domain.Exceptions;

public class InvalidStateTransitionException : OrbitalGuardianDomainException
{
    public InvalidStateTransitionException(string fromState, string toState)
        : base($"Cannot transition from {fromState} to {toState}.") { }
}
