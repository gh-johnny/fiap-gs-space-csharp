using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Queries;

public class GetSpaceObjectByIdQueryHandler : IQueryHandler<GetSpaceObjectByIdQuery, SpaceObjectResponse>
{
    private readonly ISpaceObjectRepository _repo;

    public GetSpaceObjectByIdQueryHandler(ISpaceObjectRepository repo) => _repo = repo;

    public async Task<SpaceObjectResponse> HandleAsync(GetSpaceObjectByIdQuery query, CancellationToken ct)
    {
        var obj = await _repo.GetByIdAsync(query.Id, ct)
            ?? throw new SpaceObjectNotFoundException(query.Id.ToString());
        return GetSpaceObjectsQueryHandler.MapToResponse(obj);
    }
}
