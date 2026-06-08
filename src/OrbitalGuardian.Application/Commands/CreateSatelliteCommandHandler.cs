using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Application.Commands;

public class CreateSatelliteCommandHandler : ICommandHandler<CreateSatelliteCommand, SpaceObjectResponse>
{
    private readonly ISpaceObjectRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public CreateSatelliteCommandHandler(ISpaceObjectRepository repo, IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<SpaceObjectResponse> HandleAsync(CreateSatelliteCommand command, CancellationToken ct)
    {
        var orbitalElements = OrbitalElements.Create(
            command.Inclination, command.Eccentricity, command.MeanMotion,
            command.RightAscension, command.ArgumentOfPerigee, command.MeanAnomaly);

        var satellite = Satellite.Create(
            command.Name, command.NoradId, command.LaunchDate,
            orbitalElements, command.Operator, command.MissionType, command.MassKg);

        await _repo.AddAsync(satellite, ct);
        await _dispatcher.DispatchAsync(satellite.GetDomainEvents(), ct);
        satellite.ClearDomainEvents();

        OrbitalLogger.LogInfo("CreateSatellite", $"Satellite '{satellite.Name}' created with ID {satellite.Id}");

        return MapToResponse(satellite);
    }

    private static SpaceObjectResponse MapToResponse(SpaceObject obj) => new()
    {
        Id = obj.Id, Name = obj.Name, NoradId = obj.NoradId,
        Type = obj.Type.ToString(), LaunchDate = obj.LaunchDate, IsActive = obj.IsActive,
        Inclination = obj.OrbitalElements.Inclination,
        Eccentricity = obj.OrbitalElements.Eccentricity,
        MeanMotion = obj.OrbitalElements.MeanMotion,
        RightAscension = obj.OrbitalElements.RightAscension,
        ArgumentOfPerigee = obj.OrbitalElements.ArgumentOfPerigee,
        MeanAnomaly = obj.OrbitalElements.MeanAnomaly,
        TelemetryReadings = obj.TelemetryReadings.Select(t => new TelemetryReadingResponse
        {
            Id = t.Id, SpaceObjectId = t.SpaceObjectId, Timestamp = t.Timestamp,
            X = t.StateVector.Position.X, Y = t.StateVector.Position.Y, Z = t.StateVector.Position.Z,
            Vx = t.StateVector.Velocity.X, Vy = t.StateVector.Velocity.Y, Vz = t.StateVector.Velocity.Z,
            UncertaintyKm = t.StateVector.PositionalUncertaintyKm
        })
    };
}
