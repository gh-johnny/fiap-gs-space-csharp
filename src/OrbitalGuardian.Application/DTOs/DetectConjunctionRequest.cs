namespace OrbitalGuardian.Application.DTOs;

public class DetectConjunctionRequest
{
    public Guid PrimaryObjectId { get; set; }
    public Guid SecondaryObjectId { get; set; }
    public DateTime PredictedTcaUtc { get; set; }
}
