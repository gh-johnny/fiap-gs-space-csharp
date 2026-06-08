namespace OrbitalGuardian.Application.DTOs;

public class AddTelemetryRequest
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Vx { get; set; }
    public double Vy { get; set; }
    public double Vz { get; set; }
    public double UncertaintyKm { get; set; }
    public DateTime Timestamp { get; set; }
}
