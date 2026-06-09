using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;

namespace OrbitalGuardian.API.Controllers;

[Route("api/conjunctions")]
[ApiController]
[Authorize]
public class ConjunctionsController : ControllerBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    public ConjunctionsController(ICommandDispatcher commands, IQueryDispatcher queries)
    {
        _commands = commands;
        _queries = queries;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<ConjunctionEventResponse>> Detect([FromBody] DetectConjunctionRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new DetectConjunctionCommand(request.PrimaryObjectId, request.SecondaryObjectId, request.PredictedTcaUtc), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetActive(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetActiveConjunctionsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<ConjunctionEventResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionByIdQuery(id), ct);
        return Ok(result);
    }
}
