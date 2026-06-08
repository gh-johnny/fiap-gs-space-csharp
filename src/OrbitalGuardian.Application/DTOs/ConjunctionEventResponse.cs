namespace OrbitalGuardian.Application.DTOs;

public class ConjunctionEventResponse
{
    public Guid Id { get; set; }
    public Guid PrimaryObjectId { get; set; }
    public Guid SecondaryObjectId { get; set; }
    public DateTime DetectedAt { get; set; }
    public DateTime PredictedTcaUtc { get; set; }
    public double MissDistanceKm { get; set; }
    public double TimeToClosestApproachSeconds { get; set; }
    public double CollisionProbability { get; set; }
    public string Status { get; set; } = null!;
    public IEnumerable<AlertResponse> Alerts { get; set; } = [];
}
