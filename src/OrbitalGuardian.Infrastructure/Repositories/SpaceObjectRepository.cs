using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.SpaceObjects;
using OrbitalGuardian.Infrastructure.Persistence;
using Polly;

namespace OrbitalGuardian.Infrastructure.Repositories;

public class SpaceObjectRepository : ISpaceObjectRepository
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly IAsyncPolicy _policy;

    public SpaceObjectRepository(OrbitalGuardianDbContext context)
    {
        _context = context;
        _policy = Policy.WrapAsync(
            RepositoryPolicy.CreateRetryPolicy(),
            RepositoryPolicy.CreateTimeoutPolicy());
    }

    public async Task<SpaceObject?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _policy.ExecuteAsync(ct => _context.SpaceObjects
            .Include("_telemetryReadings")
            .FirstOrDefaultAsync(x => x.Id == id, ct), ct);

    public async Task<SpaceObjectCollection> GetAllAsync(CancellationToken ct)
    {
        var results = await _policy.ExecuteAsync(ct =>
            _context.SpaceObjects
                .Include("_telemetryReadings")
                .ToListAsync(ct), ct);
        return new SpaceObjectCollection(results);
    }

    public async Task AddAsync(SpaceObject spaceObject, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            await _context.SpaceObjects.AddAsync(spaceObject, ct);
            await _context.SaveChangesAsync(ct);
        }, ct);

    public async Task UpdateAsync(SpaceObject spaceObject, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            _context.SpaceObjects.Update(spaceObject);
            await _context.SaveChangesAsync(ct);
        }, ct);
}
