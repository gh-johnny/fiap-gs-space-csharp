using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;

namespace OrbitalGuardian.Application.Queries;

public class GetConjunctionsQueryHandler : IQueryHandler<GetConjunctionsQuery, IReadOnlyList<ConjunctionEventResponse>>
{
    private readonly IConjunctionEventRepository _repo;

    public GetConjunctionsQueryHandler(IConjunctionEventRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ConjunctionEventResponse>> HandleAsync(GetConjunctionsQuery query, CancellationToken ct)
    {
        var all = await _repo.GetAllAsync(ct);
        return all.Select(MapToResponse).ToList();
    }

    internal static ConjunctionEventResponse MapToResponse(ConjunctionEvent c) => new()
    {
        Id = c.Id, PrimaryObjectId = c.PrimaryObjectId, SecondaryObjectId = c.SecondaryObjectId,
        DetectedAt = c.DetectedAt, PredictedTcaUtc = c.PredictedTcaUtc,
        MissDistanceKm = c.MissDistance.ValueKm,
        TimeToClosestApproachSeconds = c.MissDistance.TimeToClosestApproachSeconds,
        CollisionProbability = c.CollisionProbability.Value,
        Status = c.Status.ToString(),
        Alerts = c.Alerts.Select(a => new AlertResponse
        {
            Id = a.Id, ConjunctionEventId = a.ConjunctionEventId,
            Severity = a.Severity, Message = a.Message,
            IssuedAt = a.IssuedAt, AcknowledgedAt = a.AcknowledgedAt, Status = a.Status
        })
    };
}
