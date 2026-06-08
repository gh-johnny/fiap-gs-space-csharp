namespace OrbitalGuardian.Application.DTOs;

public class SpaceObjectTleDto
{
    public string Name { get; set; } = null!;
    public string NoradId { get; set; } = null!;
    public string ObjectType { get; set; } = null!;
    public string Line1 { get; set; } = null!;
    public string Line2 { get; set; } = null!;
}
