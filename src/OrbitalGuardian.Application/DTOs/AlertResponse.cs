using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Application.DTOs;

public class AlertResponse
{
    public Guid Id { get; set; }
    public Guid ConjunctionEventId { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public AlertStatus Status { get; set; }
}
