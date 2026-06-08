using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Application.Commands;

public class CreateDebrisCommandHandler : ICommandHandler<CreateDebrisCommand, SpaceObjectResponse>
{
    private readonly ISpaceObjectRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public CreateDebrisCommandHandler(ISpaceObjectRepository repo, IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<SpaceObjectResponse> HandleAsync(CreateDebrisCommand command, CancellationToken ct)
    {
        var orbitalElements = OrbitalElements.Create(
            command.Inclination, command.Eccentricity, command.MeanMotion,
            command.RightAscension, command.ArgumentOfPerigee, command.MeanAnomaly);

        var debris = SpaceDebris.Create(
            command.Name, command.NoradId, command.LaunchDate,
            orbitalElements, command.OriginObject, command.EstimatedSizeM);

        await _repo.AddAsync(debris, ct);
        await _dispatcher.DispatchAsync(debris.GetDomainEvents(), ct);
        debris.ClearDomainEvents();

        OrbitalLogger.LogInfo("CreateDebris", $"Debris '{debris.Name}' created with ID {debris.Id}");

        return MapToResponse(debris);
    }

    private static SpaceObjectResponse MapToResponse(SpaceObject obj) => new()
    {
        Id = obj.Id, Name = obj.Name, NoradId = obj.NoradId,
        Type = obj.Type.ToString(), LaunchDate = obj.LaunchDate, IsActive = obj.IsActive,
        Inclination = obj.OrbitalElements.Inclination, Eccentricity = obj.OrbitalElements.Eccentricity,
        MeanMotion = obj.OrbitalElements.MeanMotion, RightAscension = obj.OrbitalElements.RightAscension,
        ArgumentOfPerigee = obj.OrbitalElements.ArgumentOfPerigee, MeanAnomaly = obj.OrbitalElements.MeanAnomaly,
        TelemetryReadings = []
    };
}
