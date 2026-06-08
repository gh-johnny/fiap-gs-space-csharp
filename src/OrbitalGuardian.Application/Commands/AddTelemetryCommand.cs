using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record AddTelemetryCommand(
    Guid SpaceObjectId,
    double X, double Y, double Z,
    double Vx, double Vy, double Vz,
    double UncertaintyKm,
    DateTime Timestamp
) : ICommand<TelemetryReadingResponse>;
