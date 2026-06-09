using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;

namespace OrbitalGuardian.API.Controllers;

[Route("api/alerts")]
[ApiController]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    public AlertsController(ICommandDispatcher commands, IQueryDispatcher queries)
    {
        _commands = commands;
        _queries = queries;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionsQuery(), ct);
        return Ok(result.SelectMany(c => c.Alerts));
    }

    [HttpPatch("{id:guid}/acknowledge")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<AlertResponse>> Acknowledge(Guid id, [FromBody] AcknowledgeAlertRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new AcknowledgeAlertCommand(id), ct);
        return Ok(result);
    }
}
