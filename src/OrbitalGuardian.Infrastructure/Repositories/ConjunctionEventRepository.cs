using Microsoft.EntityFrameworkCore;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Enums;
using OrbitalGuardian.Infrastructure.Persistence;
using Polly;

namespace OrbitalGuardian.Infrastructure.Repositories;

public class ConjunctionEventRepository : IConjunctionEventRepository
{
    private readonly OrbitalGuardianDbContext _context;
    private readonly IAsyncPolicy _policy;

    public ConjunctionEventRepository(OrbitalGuardianDbContext context)
    {
        _context = context;
        _policy = Policy.WrapAsync(
            RepositoryPolicy.CreateRetryPolicy(),
            RepositoryPolicy.CreateTimeoutPolicy());
    }

    public async Task<ConjunctionEvent?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _policy.ExecuteAsync(ct => _context.ConjunctionEvents
            .Include("_alerts")
            .FirstOrDefaultAsync(x => x.Id == id, ct), ct);

    public async Task<ConjunctionEventCollection> GetAllAsync(CancellationToken ct)
    {
        var results = await _policy.ExecuteAsync(ct =>
            _context.ConjunctionEvents
                .Include("_alerts")
                .ToListAsync(ct), ct);
        return new ConjunctionEventCollection(results);
    }

    public async Task<ConjunctionEventCollection> GetActiveAsync(CancellationToken ct)
    {
        var results = await _policy.ExecuteAsync(ct =>
            _context.ConjunctionEvents
                .Include("_alerts")
                .Where(x => x.Status == ConjunctionStatus.Active)
                .ToListAsync(ct), ct);
        return new ConjunctionEventCollection(results);
    }

    public async Task AddAsync(ConjunctionEvent conjunctionEvent, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            await _context.ConjunctionEvents.AddAsync(conjunctionEvent, ct);
            await _context.SaveChangesAsync(ct);
        }, ct);

    public async Task UpdateAsync(ConjunctionEvent conjunctionEvent, CancellationToken ct) =>
        await _policy.ExecuteAsync(async ct =>
        {
            _context.ConjunctionEvents.Update(conjunctionEvent);
            await _context.SaveChangesAsync(ct);
        }, ct);
}
