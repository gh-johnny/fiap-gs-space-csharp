using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record CreateSatelliteCommand(
    string Name, string NoradId, DateTime LaunchDate,
    string Operator, string MissionType, double MassKg,
    double Inclination, double Eccentricity, double MeanMotion,
    double RightAscension, double ArgumentOfPerigee, double MeanAnomaly
) : ICommand<SpaceObjectResponse>;
