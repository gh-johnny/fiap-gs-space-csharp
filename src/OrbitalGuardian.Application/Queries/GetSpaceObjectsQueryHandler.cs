using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Queries;

public class GetSpaceObjectsQueryHandler : IQueryHandler<GetSpaceObjectsQuery, IReadOnlyList<SpaceObjectResponse>>
{
    private readonly ISpaceObjectRepository _repo;

    public GetSpaceObjectsQueryHandler(ISpaceObjectRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SpaceObjectResponse>> HandleAsync(GetSpaceObjectsQuery query, CancellationToken ct)
    {
        OrbitalLogger.LogDebug("GetSpaceObjects", "Fetching all space objects");
        var all = await _repo.GetAllAsync(ct);
        return all.Select(MapToResponse).ToList();
    }

    internal static SpaceObjectResponse MapToResponse(SpaceObject obj) => new()
    {
        Id = obj.Id, Name = obj.Name, NoradId = obj.NoradId,
        Type = obj.Type.ToString(), LaunchDate = obj.LaunchDate, IsActive = obj.IsActive,
        Inclination = obj.OrbitalElements.Inclination, Eccentricity = obj.OrbitalElements.Eccentricity,
        MeanMotion = obj.OrbitalElements.MeanMotion, RightAscension = obj.OrbitalElements.RightAscension,
        ArgumentOfPerigee = obj.OrbitalElements.ArgumentOfPerigee, MeanAnomaly = obj.OrbitalElements.MeanAnomaly,
        TelemetryReadings = obj.TelemetryReadings.Select(t => new TelemetryReadingResponse
        {
            Id = t.Id, SpaceObjectId = t.SpaceObjectId, Timestamp = t.Timestamp,
            X = t.StateVector.Position.X, Y = t.StateVector.Position.Y, Z = t.StateVector.Position.Z,
            Vx = t.StateVector.Velocity.X, Vy = t.StateVector.Velocity.Y, Vz = t.StateVector.Velocity.Z,
            UncertaintyKm = t.StateVector.PositionalUncertaintyKm
        })
    };
}
