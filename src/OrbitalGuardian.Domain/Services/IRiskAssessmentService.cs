using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Domain.Services;

/// <summary>
/// Computes the collision probability and miss distance between two space objects
/// based on their most recent telemetry readings.
/// </summary>
public interface IRiskAssessmentService
{
    CollisionProbability Assess(SpaceObject primary, SpaceObject secondary, out MissDistance missDistance);
}
