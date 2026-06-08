using System.ComponentModel.DataAnnotations;

namespace OrbitalGuardian.Application.DTOs;

public class CreateDebrisRequest
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string NoradId { get; set; } = null!;
    public DateTime LaunchDate { get; set; }
    [Required] public string OriginObject { get; set; } = null!;
    public double EstimatedSizeM { get; set; }
    public double Inclination { get; set; }
    public double Eccentricity { get; set; }
    public double MeanMotion { get; set; }
    public double RightAscension { get; set; }
    public double ArgumentOfPerigee { get; set; }
    public double MeanAnomaly { get; set; }
}
