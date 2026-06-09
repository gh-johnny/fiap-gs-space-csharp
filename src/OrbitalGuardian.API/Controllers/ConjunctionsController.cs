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

    /// <summary>Registra um novo evento de conjunção entre dois objetos espaciais. Requer papel Admin ou Operator.</summary>
    /// <response code="201">Evento de conjunção criado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(ConjunctionEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ConjunctionEventResponse>> Detect([FromBody] DetectConjunctionRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new DetectConjunctionCommand(request.PrimaryObjectId, request.SecondaryObjectId, request.PredictedTcaUtc), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Lista todos os eventos de conjunção registrados. Requer papel Admin, Operator ou Analyst.</summary>
    /// <response code="200">Lista de eventos retornada com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(IReadOnlyList<ConjunctionEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Lista apenas os eventos de conjunção com status ativo. Requer papel Admin, Operator ou Analyst.</summary>
    /// <response code="200">Lista de eventos ativos retornada com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpGet("active")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(IReadOnlyList<ConjunctionEventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetActive(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetActiveConjunctionsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Busca um evento de conjunção pelo seu identificador único. Requer papel Admin, Operator ou Analyst.</summary>
    /// <param name="id">ID do evento de conjunção.</param>
    /// <response code="200">Evento de conjunção encontrado e retornado com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Evento de conjunção não encontrado.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(ConjunctionEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConjunctionEventResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionByIdQuery(id), ct);
        return Ok(result);
    }

    /// <summary>Remove permanentemente um evento de conjunção. Requer papel Admin.</summary>
    /// <param name="id">ID do evento de conjunção a ser removido.</param>
    /// <response code="204">Evento de conjunção removido com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Evento de conjunção não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _commands.DispatchAsync(new DeleteConjunctionEventCommand(id), ct);
        return NoContent();
    }
}
