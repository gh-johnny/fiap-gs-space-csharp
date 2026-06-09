using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Infrastructure.Gateways;

public class SpaceTrackTleGateway : ITleDataGateway
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SpaceTrackTleGateway(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public Task<IReadOnlyList<SpaceObjectTleDto>> FetchTleDataAsync(CancellationToken ct) =>
        throw new NotImplementedException(
            "Configure Space-Track credentials in appsettings to use the real gateway.");
}
