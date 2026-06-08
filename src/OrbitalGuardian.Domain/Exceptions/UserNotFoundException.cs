namespace OrbitalGuardian.Domain.Exceptions;

public class UserNotFoundException : OrbitalGuardianDomainException
{
    public UserNotFoundException(string identifier)
        : base($"User '{identifier}' was not found.") { }
}
