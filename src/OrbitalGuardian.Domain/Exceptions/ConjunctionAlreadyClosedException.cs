namespace OrbitalGuardian.Domain.Exceptions;

public class ConjunctionAlreadyClosedException : OrbitalGuardianDomainException
{
    public ConjunctionAlreadyClosedException(string message) : base(message) { }
}
