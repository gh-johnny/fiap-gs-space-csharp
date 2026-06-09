namespace OrbitalGuardian.API.Settings;

public class OrbitalGuardianSettings
{
    public int PollyRetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 10;
}
