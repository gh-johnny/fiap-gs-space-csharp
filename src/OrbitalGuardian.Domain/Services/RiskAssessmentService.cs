using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Services;

public class RiskAssessmentService : IRiskAssessmentService
{
    public CollisionProbability Assess(SpaceObject primary, SpaceObject secondary, out MissDistance missDistance)
    {
        var primaryTelemetry = primary.TelemetryReadings.Latest()
            ?? throw new InvalidStateVectorException($"Space object '{primary.NoradId}' has no telemetry data.");
        var secondaryTelemetry = secondary.TelemetryReadings.Latest()
            ?? throw new InvalidStateVectorException($"Space object '{secondary.NoradId}' has no telemetry data.");

        var distKm = primaryTelemetry.StateVector.Position
            .DistanceTo(secondaryTelemetry.StateVector.Position);

        var tcaSeconds = Math.Abs(
            (primaryTelemetry.Timestamp - secondaryTelemetry.Timestamp).TotalSeconds);

        missDistance = MissDistance.Create(distKm, tcaSeconds);

        // Simplified academic Gaussian Pc calculation
        var combinedUncertainty = primaryTelemetry.StateVector.PositionalUncertaintyKm
                                + secondaryTelemetry.StateVector.PositionalUncertaintyKm;

        double pc;
        if (combinedUncertainty <= 0 || distKm <= 0)
        {
            pc = 0;
        }
        else
        {
            var sigma = combinedUncertainty;
            var exponent = -(distKm * distKm) / (2 * sigma * sigma);
            pc = Math.Exp(exponent) / (Math.Sqrt(2 * Math.PI) * sigma);
            pc = Math.Min(pc, 1.0);
        }

        return CollisionProbability.Create(pc);
    }
}
