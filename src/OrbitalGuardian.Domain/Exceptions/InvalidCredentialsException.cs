namespace OrbitalGuardian.Domain.Exceptions;

public class InvalidCredentialsException : OrbitalGuardianDomainException
{
    public InvalidCredentialsException() : base("Invalid email or password.") { }
}
