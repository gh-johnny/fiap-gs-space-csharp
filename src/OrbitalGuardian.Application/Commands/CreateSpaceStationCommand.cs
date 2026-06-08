using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record CreateSpaceStationCommand(
    string Name, string NoradId, DateTime LaunchDate,
    string Agency, int CrewCapacity,
    double Inclination, double Eccentricity, double MeanMotion,
    double RightAscension, double ArgumentOfPerigee, double MeanAnomaly
) : ICommand<SpaceObjectResponse>;
