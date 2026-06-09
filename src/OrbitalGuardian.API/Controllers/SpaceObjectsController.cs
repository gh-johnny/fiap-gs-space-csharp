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

    [HttpPost("satellite")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<SpaceObjectResponse>> CreateSatellite([FromBody] CreateSatelliteRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateSatelliteCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.Operator, request.MissionType, request.MassKg,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("debris")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<SpaceObjectResponse>> CreateDebris([FromBody] CreateDebrisRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateDebrisCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.OriginObject, request.EstimatedSizeM,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("station")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<SpaceObjectResponse>> CreateStation([FromBody] CreateSpaceStationRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new CreateSpaceStationCommand(
            request.Name, request.NoradId, request.LaunchDate,
            request.Agency, request.CrewCapacity,
            request.Inclination, request.Eccentricity, request.MeanMotion,
            request.RightAscension, request.ArgumentOfPerigee, request.MeanAnomaly), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<IReadOnlyList<SpaceObjectResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetSpaceObjectsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Operator,Analyst")]
    public async Task<ActionResult<SpaceObjectResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetSpaceObjectByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/telemetry")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<TelemetryReadingResponse>> AddTelemetry(
        Guid id, [FromBody] AddTelemetryRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new AddTelemetryCommand(
            id, request.X, request.Y, request.Z,
            request.Vx, request.Vy, request.Vz,
            request.UncertaintyKm, request.Timestamp), ct);
        return CreatedAtAction(nameof(GetById), new { id }, result);
    }

    [HttpPost("import-tle")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<SpaceObjectResponse>>> ImportTle(CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new ImportTleDataCommand(), ct);
        return Ok(result);
    }

    /// <summary>Deactivates a space object (soft delete). Requires Admin or Operator role.</summary>
    /// <param name="id">Space object ID.</param>
    /// <response code="204">Space object deactivated successfully.</response>
    /// <response code="404">Space object not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Operator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _commands.DispatchAsync(new DeleteSpaceObjectCommand(id), ct);
        return NoContent();
    }
}
