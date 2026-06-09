using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Infrastructure.Persistence;
using OrbitalGuardian.Infrastructure.Repositories;

namespace OrbitalGuardian.Tests.Infrastructure.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly UserRepository _repo;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrbitalGuardianDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new OrbitalGuardianDbContext(options);
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ThenGetByEmailAsync_ReturnsUser()
    {
        var user = User.Create("jo@test.com", "hash", "Jo", UserRole.Admin);
        await _repo.AddAsync(user, CancellationToken.None);
        var found = await _repo.GetByEmailAsync("jo@test.com", CancellationToken.None);
        found.Should().NotBeNull();
        found!.FullName.Should().Be("Jo");
    }

    [Fact]
    public async Task GetByEmailAsync_CaseInsensitive_ReturnsUser()
    {
        var user = User.Create("Admin@X.com", "hash", "Admin", UserRole.Admin);
        await _repo.AddAsync(user, CancellationToken.None);
        var found = await _repo.GetByEmailAsync("admin@x.com", CancellationToken.None);
        found.Should().NotBeNull();
    }

    public void Dispose() => _context.Dispose();
}
