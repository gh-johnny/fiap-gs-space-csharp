namespace OrbitalGuardian.Domain.Exceptions;

public class InvalidStateVectorException : OrbitalGuardianDomainException
{
    public InvalidStateVectorException(string message) : base(message) { }
}
