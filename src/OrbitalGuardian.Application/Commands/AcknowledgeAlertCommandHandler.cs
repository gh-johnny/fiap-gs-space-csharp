using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Commands;

public class AcknowledgeAlertCommandHandler : ICommandHandler<AcknowledgeAlertCommand, AlertResponse>
{
    private readonly IConjunctionEventRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public AcknowledgeAlertCommandHandler(IConjunctionEventRepository repo, IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<AlertResponse> HandleAsync(AcknowledgeAlertCommand command, CancellationToken ct)
    {
        var conjunctions = await _repo.GetAllAsync(ct);
        var conjunction = conjunctions.FirstOrDefault(c => c.Alerts.FindById(command.AlertId) is not null)
            ?? throw new InvalidOperationException($"Alert {command.AlertId} not found.");

        conjunction.AcknowledgeAlert(command.AlertId, DateTime.UtcNow);

        await _repo.UpdateAsync(conjunction, ct);
        await _dispatcher.DispatchAsync(conjunction.GetDomainEvents(), ct);
        conjunction.ClearDomainEvents();

        OrbitalLogger.LogInfo("AcknowledgeAlert", $"Alert {command.AlertId} acknowledged");

        var alert = conjunction.Alerts.FindById(command.AlertId)!;
        return new AlertResponse
        {
            Id = alert.Id, ConjunctionEventId = alert.ConjunctionEventId,
            Severity = alert.Severity, Message = alert.Message,
            IssuedAt = alert.IssuedAt, AcknowledgedAt = alert.AcknowledgedAt, Status = alert.Status
        };
    }
}
