using OrbitalGuardian.Domain.Aggregates.Conjunctions;

namespace OrbitalGuardian.Application.Interfaces;

public interface IConjunctionEventRepository
{
    Task<ConjunctionEvent?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ConjunctionEventCollection> GetAllAsync(CancellationToken ct);
    Task<ConjunctionEventCollection> GetActiveAsync(CancellationToken ct);
    Task AddAsync(ConjunctionEvent conjunctionEvent, CancellationToken ct);
    Task UpdateAsync(ConjunctionEvent conjunctionEvent, CancellationToken ct);
}
