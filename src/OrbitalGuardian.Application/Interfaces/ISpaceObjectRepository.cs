using OrbitalGuardian.Domain.Aggregates.SpaceObjects;

namespace OrbitalGuardian.Application.Interfaces;

public interface ISpaceObjectRepository
{
    Task<SpaceObject?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<SpaceObjectCollection> GetAllAsync(CancellationToken ct);
    Task AddAsync(SpaceObject spaceObject, CancellationToken ct);
    Task UpdateAsync(SpaceObject spaceObject, CancellationToken ct);
}
