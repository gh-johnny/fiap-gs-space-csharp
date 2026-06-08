namespace OrbitalGuardian.Domain.Exceptions;

public class SpaceObjectNotFoundException : OrbitalGuardianDomainException
{
    public SpaceObjectNotFoundException(string identifier)
        : base($"Space object '{identifier}' was not found.") { }
}
