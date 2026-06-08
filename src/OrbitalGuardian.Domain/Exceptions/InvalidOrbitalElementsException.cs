namespace OrbitalGuardian.Domain.Exceptions;

public class InvalidOrbitalElementsException : OrbitalGuardianDomainException
{
    public InvalidOrbitalElementsException(string message) : base(message) { }
}
