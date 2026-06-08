namespace OrbitalGuardian.Domain.Exceptions;

public class DuplicateEmailException : OrbitalGuardianDomainException
{
    public DuplicateEmailException(string email)
        : base($"Email '{email}' is already registered.") { }
}
