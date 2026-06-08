using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public class GetActiveConjunctionsQueryHandler : IQueryHandler<GetActiveConjunctionsQuery, IReadOnlyList<ConjunctionEventResponse>>
{
    private readonly IConjunctionEventRepository _repo;

    public GetActiveConjunctionsQueryHandler(IConjunctionEventRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ConjunctionEventResponse>> HandleAsync(GetActiveConjunctionsQuery query, CancellationToken ct)
    {
        var active = await _repo.GetActiveAsync(ct);
        return active.Select(GetConjunctionsQueryHandler.MapToResponse).ToList();
    }
}
