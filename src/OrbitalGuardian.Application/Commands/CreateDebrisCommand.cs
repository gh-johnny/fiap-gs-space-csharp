using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record CreateDebrisCommand(
    string Name, string NoradId, DateTime LaunchDate,
    string OriginObject, double EstimatedSizeM,
    double Inclination, double Eccentricity, double MeanMotion,
    double RightAscension, double ArgumentOfPerigee, double MeanAnomaly
) : ICommand<SpaceObjectResponse>;
