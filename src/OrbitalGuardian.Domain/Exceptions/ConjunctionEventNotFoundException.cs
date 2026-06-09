namespace OrbitalGuardian.Domain.Exceptions;

public class ConjunctionEventNotFoundException : OrbitalGuardianDomainException
{
    public ConjunctionEventNotFoundException(string identifier)
        : base($"Conjunction event '{identifier}' was not found.") { }
}
