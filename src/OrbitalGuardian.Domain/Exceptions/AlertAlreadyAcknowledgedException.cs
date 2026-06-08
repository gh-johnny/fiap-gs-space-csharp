namespace OrbitalGuardian.Domain.Exceptions;

public class AlertAlreadyAcknowledgedException : OrbitalGuardianDomainException
{
    public AlertAlreadyAcknowledgedException(string message) : base(message) { }
}
