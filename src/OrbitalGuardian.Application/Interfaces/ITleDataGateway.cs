using OrbitalGuardian.Application.DTOs;

namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Fetches raw TLE data from an external orbital catalog source.</summary>
public interface ITleDataGateway
{
    Task<IReadOnlyList<SpaceObjectTleDto>> FetchTleDataAsync(CancellationToken ct);
}
