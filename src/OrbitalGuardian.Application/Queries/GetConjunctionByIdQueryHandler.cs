using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public class GetConjunctionByIdQueryHandler : IQueryHandler<GetConjunctionByIdQuery, ConjunctionEventResponse>
{
    private readonly IConjunctionEventRepository _repo;

    public GetConjunctionByIdQueryHandler(IConjunctionEventRepository repo) => _repo = repo;

    public async Task<ConjunctionEventResponse> HandleAsync(GetConjunctionByIdQuery query, CancellationToken ct)
    {
        var conj = await _repo.GetByIdAsync(query.Id, ct)
            ?? throw new InvalidOperationException($"Conjunction {query.Id} not found.");
        return GetConjunctionsQueryHandler.MapToResponse(conj);
    }
}
