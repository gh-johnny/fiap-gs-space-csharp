using System.Globalization;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Shared;

/// <summary>
/// Parses NORAD Two-Line Element (TLE) sets into OrbitalElements.
/// TLE is the standard format for representing satellite orbital parameters.
/// </summary>
public static class TleParser
{
    /// <summary>
    /// Parses a TLE line pair into an OrbitalElements value object.
    /// Extracts fields from fixed character positions per the NORAD TLE specification.
    /// Throws <see cref="InvalidOrbitalElementsException"/> if either line is null or shorter than 69 characters.
    /// </summary>
    public static OrbitalElements Parse(string line1, string line2)
    {
        if (string.IsNullOrWhiteSpace(line1) || string.IsNullOrWhiteSpace(line2))
            throw new InvalidOrbitalElementsException("TLE lines cannot be null or empty.");
        if (line1.Length < 69 || line2.Length < 69)
            throw new InvalidOrbitalElementsException("TLE lines must be at least 69 characters long.");

        var inclination       = ParseTleDouble(line2[8..16]);
        var raan              = ParseTleDouble(line2[17..25]);
        var eccentricity      = ParseTleDouble("0." + line2[26..33].Trim());
        var argumentOfPerigee = ParseTleDouble(line2[34..42]);
        var meanAnomaly       = ParseTleDouble(line2[43..51]);
        var meanMotion        = ParseTleDouble(line2[52..63]);

        return OrbitalElements.Create(inclination, eccentricity, meanMotion, raan, argumentOfPerigee, meanAnomaly);
    }

    private static double ParseTleDouble(string raw)
    {
        try
        {
            return double.Parse(raw.Trim(), CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new InvalidOrbitalElementsException($"Cannot parse TLE field value: '{raw}'.");
        }
    }
}
