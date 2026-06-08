using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Conjunctions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Services;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Commands;

public class DetectConjunctionCommandHandler : ICommandHandler<DetectConjunctionCommand, ConjunctionEventResponse>
{
    private readonly ISpaceObjectRepository _spaceRepo;
    private readonly IConjunctionEventRepository _conjRepo;
    private readonly IRiskAssessmentService _riskService;
    private readonly IDomainEventDispatcher _dispatcher;

    public DetectConjunctionCommandHandler(
        ISpaceObjectRepository spaceRepo,
        IConjunctionEventRepository conjRepo,
        IRiskAssessmentService riskService,
        IDomainEventDispatcher dispatcher)
    {
        _spaceRepo = spaceRepo;
        _conjRepo = conjRepo;
        _riskService = riskService;
        _dispatcher = dispatcher;
    }

    public async Task<ConjunctionEventResponse> HandleAsync(DetectConjunctionCommand command, CancellationToken ct)
    {
        var primary = await _spaceRepo.GetByIdAsync(command.PrimaryObjectId, ct)
            ?? throw new SpaceObjectNotFoundException(command.PrimaryObjectId.ToString());
        var secondary = await _spaceRepo.GetByIdAsync(command.SecondaryObjectId, ct)
            ?? throw new SpaceObjectNotFoundException(command.SecondaryObjectId.ToString());

        var pc = _riskService.Assess(primary, secondary, out var missDistance);

        var conjunction = new ConjunctionEventBuilder()
            .WithPrimaryObject(primary.Id)
            .WithSecondaryObject(secondary.Id)
            .WithPredictedTca(command.PredictedTcaUtc)
            .WithMissDistance(missDistance)
            .WithCollisionProbability(pc)
            .Build();

        await _conjRepo.AddAsync(conjunction, ct);
        await _dispatcher.DispatchAsync(conjunction.GetDomainEvents(), ct);
        conjunction.ClearDomainEvents();

        OrbitalLogger.LogInfo("DetectConjunction", $"Conjunction detected: Pc={pc.Value:E2}, Miss={missDistance.ValueKm:F2}km");

        return MapToResponse(conjunction);
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
