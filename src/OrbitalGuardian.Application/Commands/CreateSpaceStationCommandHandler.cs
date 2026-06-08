using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Application.Commands;

public class CreateSpaceStationCommandHandler : ICommandHandler<CreateSpaceStationCommand, SpaceObjectResponse>
{
    private readonly ISpaceObjectRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public CreateSpaceStationCommandHandler(ISpaceObjectRepository repo, IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<SpaceObjectResponse> HandleAsync(CreateSpaceStationCommand command, CancellationToken ct)
    {
        var orbitalElements = OrbitalElements.Create(
            command.Inclination, command.Eccentricity, command.MeanMotion,
            command.RightAscension, command.ArgumentOfPerigee, command.MeanAnomaly);

        var station = SpaceStation.Create(
            command.Name, command.NoradId, command.LaunchDate,
            orbitalElements, command.CrewCapacity, command.Agency);

        await _repo.AddAsync(station, ct);
        await _dispatcher.DispatchAsync(station.GetDomainEvents(), ct);
        station.ClearDomainEvents();

        OrbitalLogger.LogInfo("CreateSpaceStation", $"Station '{station.Name}' created with ID {station.Id}");

        return new SpaceObjectResponse
        {
            Id = station.Id, Name = station.Name, NoradId = station.NoradId,
            Type = station.Type.ToString(), LaunchDate = station.LaunchDate, IsActive = station.IsActive,
            Inclination = station.OrbitalElements.Inclination, Eccentricity = station.OrbitalElements.Eccentricity,
            MeanMotion = station.OrbitalElements.MeanMotion, RightAscension = station.OrbitalElements.RightAscension,
            ArgumentOfPerigee = station.OrbitalElements.ArgumentOfPerigee, MeanAnomaly = station.OrbitalElements.MeanAnomaly,
            TelemetryReadings = []
        };
    }
}
