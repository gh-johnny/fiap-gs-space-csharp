using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Domain.ValueObjects;
using OrbitalGuardian.Infrastructure.Persistence;
using OrbitalGuardian.Infrastructure.Repositories;

namespace OrbitalGuardian.Tests.Infrastructure.Repositories;

public class ConjunctionEventRepositoryTests : IDisposable
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly ConjunctionEventRepository _repo;

    public ConjunctionEventRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrbitalGuardianDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new OrbitalGuardianDbContext(options);
        _repo = new ConjunctionEventRepository(_context);
    }

    private static ConjunctionEvent MakeActive() =>
        ConjunctionEvent.Create(Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddHours(2),
            MissDistance.Create(5, 7200),
            CollisionProbability.Create(1e-3));

    [Fact]
    public async Task GetActiveAsync_ReturnsOnlyActiveConjunctions()
    {
        var active = MakeActive();
        var resolved = MakeActive();
        resolved.Resolve();

        await _repo.AddAsync(active, CancellationToken.None);
        await _repo.AddAsync(resolved, CancellationToken.None);

        var result = await _repo.GetActiveAsync(CancellationToken.None);
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(ConjunctionStatus.Active);
    }

    [Fact]
    public async Task DeleteAsync_ExistingEvent_RemovesFromDatabase()
    {
        var conjunction = MakeActive();
        await _repo.AddAsync(conjunction, CancellationToken.None);

        await _repo.DeleteAsync(conjunction.Id, CancellationToken.None);

        var found = await _repo.GetByIdAsync(conjunction.Id, CancellationToken.None);
        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        var act = async () => await _repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);
        await act.Should().NotThrowAsync();
    }

    public void Dispose() => _context.Dispose();
}
