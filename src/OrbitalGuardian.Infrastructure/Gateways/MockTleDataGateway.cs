using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Infrastructure.Gateways;

public class MockTleDataGateway : ITleDataGateway
{
    private static readonly IReadOnlyList<SpaceObjectTleDto> MockData = new List<SpaceObjectTleDto>
    {
        new()
        {
            Name = "ISS (ZARYA)",
            NoradId = "25544",
            ObjectType = "PAYLOAD",
            Line1 = "1 25544U 98067A   24001.50000000  .00016717  00000-0  10270-3 0  9994",
            Line2 = "2 25544  51.6400 337.6640 0001770  35.5820 324.5240 15.50377579430864"
        },
        new()
        {
            Name = "HUBBLE SPACE TELESCOPE",
            NoradId = "20580",
            ObjectType = "PAYLOAD",
            Line1 = "1 20580U 90037B   24001.50000000  .00000882  00000-0  46111-4 0  9993",
            Line2 = "2 20580  28.4700 267.7020 0002719 117.5840 242.5510 15.09270408391792"
        },
        new()
        {
            Name = "NOAA 15",
            NoradId = "25338",
            ObjectType = "PAYLOAD",
            Line1 = "1 25338U 98030A   24001.50000000  .00000025  00000-0  27809-4 0  9994",
            Line2 = "2 25338  98.7460 141.5880 0010734 146.5790 213.6100 14.25898637348736"
        },
        new()
        {
            Name = "SL-8 DEB",
            NoradId = "14427",
            ObjectType = "DEBRIS",
            Line1 = "1 14427U 83031E   24001.50000000  .00000071  00000-0  87756-4 0  9998",
            Line2 = "2 14427  82.9490 305.8550 0049032 140.2280 220.1760 14.71895878235041"
        },
        new()
        {
            Name = "STARLINK-1007",
            NoradId = "44713",
            ObjectType = "PAYLOAD",
            Line1 = "1 44713U 19074A   24001.50000000  .00004164  00000-0  27765-3 0  9991",
            Line2 = "2 44713  53.0530 139.2440 0001440  83.0760 277.0520 15.06396791237412"
        }
    };

    public Task<IReadOnlyList<SpaceObjectTleDto>> FetchTleDataAsync(CancellationToken ct)
    {
        OrbitalLogger.LogInfo("MockTleGateway", $"Returning mock TLE data for {MockData.Count} objects");
        return Task.FromResult(MockData);
    }
}
