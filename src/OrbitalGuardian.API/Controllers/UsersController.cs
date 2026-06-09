using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Application.Queries;

namespace OrbitalGuardian.API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ICommandDispatcher _commands;
    private readonly IQueryDispatcher _queries;

    public UsersController(ICommandDispatcher commands, IQueryDispatcher queries)
    {
        _commands = commands;
        _queries = queries;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetUsersQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new UpdateUserCommand(id, request.FullName), ct);
        return Ok(result);
    }

    /// <summary>Deactivates a user (soft delete). Requires Admin role.</summary>
    /// <param name="id">User ID.</param>
    /// <response code="204">User deactivated successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _commands.DispatchAsync(new DeleteUserCommand(id), ct);
        return NoContent();
    }
}
