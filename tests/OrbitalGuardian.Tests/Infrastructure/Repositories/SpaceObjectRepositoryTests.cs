using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Domain.ValueObjects;
using OrbitalGuardian.Infrastructure.Persistence;
using OrbitalGuardian.Infrastructure.Repositories;

namespace OrbitalGuardian.Tests.Infrastructure.Repositories;

public class SpaceObjectRepositoryTests : IDisposable
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly SpaceObjectRepository _repo;

    public SpaceObjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrbitalGuardianDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new OrbitalGuardianDbContext(options);
        _repo = new SpaceObjectRepository(_context);
    }

    private static Satellite MakeSatellite(string norad = "25544") =>
        Satellite.Create("SAT-" + norad, norad, DateTime.UtcNow.AddYears(-5),
            OrbitalElements.Create(51.6, 0.0001, 15.5, 100, 200, 300),
            "NASA", "Research", 500);

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsSameObject()
    {
        var sat = MakeSatellite();
        await _repo.AddAsync(sat, CancellationToken.None);
        var found = await _repo.GetByIdAsync(sat.Id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.NoradId.Should().Be("25544");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAdded()
    {
        await _repo.AddAsync(MakeSatellite("1"), CancellationToken.None);
        await _repo.AddAsync(MakeSatellite("2"), CancellationToken.None);
        var all = await _repo.GetAllAsync(CancellationToken.None);
        all.Should().HaveCount(2);
    }

    public void Dispose() => _context.Dispose();
}
