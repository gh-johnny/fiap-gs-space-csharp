namespace OrbitalGuardian.Domain.Exceptions;

public class InvalidCollisionProbabilityException : OrbitalGuardianDomainException
{
    public InvalidCollisionProbabilityException(string message) : base(message) { }
}
