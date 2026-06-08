using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Commands;

public class ImportTleDataCommandHandler : ICommandHandler<ImportTleDataCommand, IReadOnlyList<SpaceObjectResponse>>
{
    private readonly ITleDataGateway _gateway;
    private readonly ISpaceObjectRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public ImportTleDataCommandHandler(ITleDataGateway gateway, ISpaceObjectRepository repo, IDomainEventDispatcher dispatcher)
    {
        _gateway = gateway;
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<IReadOnlyList<SpaceObjectResponse>> HandleAsync(ImportTleDataCommand command, CancellationToken ct)
    {
        var tleDtos = await _gateway.FetchTleDataAsync(ct);
        var all = await _repo.GetAllAsync(ct);
        var results = new List<SpaceObjectResponse>();
        int imported = 0, updated = 0;

        foreach (var dto in tleDtos)
        {
            var orbitalElements = TleParser.Parse(dto.Line1, dto.Line2);
            var existing = all.FindByNoradId(dto.NoradId);

            if (existing is not null)
            {
                existing.UpdateOrbitalElements(orbitalElements);
                await _repo.UpdateAsync(existing, ct);
                results.Add(MapToResponse(existing));
                updated++;
            }
            else
            {
                SpaceObject newObj = dto.ObjectType == "DEBRIS"
                    ? SpaceDebris.Create(dto.Name, dto.NoradId, DateTime.UtcNow, orbitalElements, "Unknown", 0)
                    : Satellite.Create(dto.Name, dto.NoradId, DateTime.UtcNow, orbitalElements, "Unknown", "Unknown", 0);

                await _repo.AddAsync(newObj, ct);
                await _dispatcher.DispatchAsync(newObj.GetDomainEvents(), ct);
                newObj.ClearDomainEvents();
                results.Add(MapToResponse(newObj));
                imported++;
            }
        }

        OrbitalLogger.LogInfo("ImportTleData", $"TLE import complete: {imported} created, {updated} updated");
        return results;
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
