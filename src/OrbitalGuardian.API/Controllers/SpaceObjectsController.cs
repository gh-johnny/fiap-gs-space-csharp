using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;

namespace OrbitalGuardian.API.Controllers;

[Route("api/space-objects")]
[ApiController]
[Authorize]
public class SpaceObjectsController : ControllerBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    public SpaceObjectsController(ICommandDispatcher commands, IQueryDispatcher queries)
    {
        _commands = commands;
        _queries = queries;
    }

    /// <summary>Cadastra um novo satélite no sistema. Requer papel Admin ou Operator.</summary>
    /// <response code="201">Satélite criado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpPost("satellite")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(SpaceObjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceObjectResponse>> CreateSatellite([FromBody] CreateSatelliteRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateSatelliteCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.Operator, request.MissionType, request.MassKg,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Cadastra um novo detrito espacial no sistema. Requer papel Admin ou Operator.</summary>
    /// <response code="201">Detrito espacial criado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpPost("debris")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(SpaceObjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceObjectResponse>> CreateDebris([FromBody] CreateDebrisRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateDebrisCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.OriginObject, request.EstimatedSizeM,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Cadastra uma nova estação espacial no sistema. Requer papel Admin ou Operator.</summary>
    /// <response code="201">Estação espacial criada com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpPost("station")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(SpaceObjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SpaceObjectResponse>> CreateStation([FromBody] CreateSpaceStationRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateSpaceStationCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.Agency, request.CrewCapacity,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Lista todos os objetos espaciais cadastrados. Requer papel Admin, Operator ou Analyst.</summary>
    /// <response code="200">Lista de objetos espaciais retornada com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(IReadOnlyList<SpaceObjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<SpaceObjectResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetSpaceObjectsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Busca um objeto espacial pelo seu identificador único. Requer papel Admin, Operator ou Analyst.</summary>
    /// <param name="id">ID do objeto espacial.</param>
    /// <response code="200">Objeto espacial encontrado e retornado com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Objeto espacial não encontrado.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    [ProducesResponseType(typeof(SpaceObjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpaceObjectResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetSpaceObjectByIdQuery(id), ct);
        return Ok(result);
    }

    /// <summary>Adiciona uma leitura de telemetria a um objeto espacial existente. Requer papel Admin ou Operator.</summary>
    /// <param name="id">ID do objeto espacial.</param>
    /// <response code="201">Leitura de telemetria registrada com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Objeto espacial não encontrado.</response>
    [HttpPost("{id:guid}/telemetry")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(typeof(TelemetryReadingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TelemetryReadingResponse>> AddTelemetry(
        Guid id, [FromBody] AddTelemetryRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new AddTelemetryCommand(
            id, request.X, request.Y, request.Z,
            request.Vx, request.Vy, request.Vz,
            request.UncertaintyKm, request.Timestamp), ct);
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }

    /// <summary>Importa objetos espaciais a partir de dados TLE obtidos de fonte externa. Requer papel Admin.</summary>
    /// <response code="200">Importação concluída. Retorna a lista de objetos criados ou atualizados.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpPost("import-tle")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<SpaceObjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<SpaceObjectResponse>>> ImportTle(CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new ImportTleDataCommand(), ct);
        return Ok(result);
    }

    /// <summary>Desativa um objeto espacial (soft delete). Requer papel Admin ou Operator.</summary>
    /// <param name="id">ID do objeto espacial a ser desativado.</param>
    /// <response code="204">Objeto espacial desativado com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Objeto espacial não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _commands.DispatchAsync(new DeleteSpaceObjectCommand(id), ct);
        return NoContent();
    }
}
