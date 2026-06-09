using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.API.Swagger;
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

    /// <summary>Lista todos os alertas gerados por eventos de conjunção. Requer papel Admin, Operator ou Analyst.</summary>
    /// <response code="200">Lista de alertas retornada com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ConjunctionEventResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetConjunctionsQuery(), ct);
        return Ok(result.SelectMany(c => c.Alerts));
    }

    /// <summary>Confirma o reconhecimento de um alerta específico. Requer papel Admin ou Operator.</summary>
    /// <param name="id">ID do alerta a ser reconhecido.</param>
    /// <response code="200">Alerta reconhecido com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [SwaggerBodyExample("{}")]
    [HttpPatch("{id:guid}/acknowledge")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertResponse>> Acknowledge(Guid id, [FromBody] AcknowledgeAlertRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new AcknowledgeAlertCommand(id), ct);
        return Ok(result);
    }
}
