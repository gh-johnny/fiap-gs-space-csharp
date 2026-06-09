namespace OrbitalGuardian.IoC;

public class OrbitalGuardianSettings
{
    public int PollyRetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 10;
}
