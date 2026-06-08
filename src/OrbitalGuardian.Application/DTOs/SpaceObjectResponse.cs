namespace OrbitalGuardian.Application.DTOs;

public class SpaceObjectResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string NoradId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime LaunchDate { get; set; }
    public bool IsActive { get; set; }
    public double Inclination { get; set; }
    public double Eccentricity { get; set; }
    public double MeanMotion { get; set; }
    public double RightAscension { get; set; }
    public double ArgumentOfPerigee { get; set; }
    public double MeanAnomaly { get; set; }
    public IEnumerable<TelemetryReadingResponse> TelemetryReadings { get; set; } = [];
}
