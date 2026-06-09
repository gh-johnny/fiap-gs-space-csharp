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

    /// <summary>Lista todos os usuários cadastrados no sistema. Requer papel Admin.</summary>
    /// <response code="200">Lista de usuários retornada com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetUsersQuery(), ct);
        return Ok(result);
    }

    /// <summary>Busca um usuário pelo seu identificador único. Requer papel Admin.</summary>
    /// <param name="id">ID do usuário.</param>
    /// <response code="200">Usuário encontrado e retornado com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _queries.DispatchAsync(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }

    /// <summary>Atualiza os dados de um usuário existente. Requer papel Admin.</summary>
    /// <param name="id">ID do usuário a ser atualizado.</param>
    /// <response code="200">Usuário atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(new UpdateUserCommand(id, request.FullName), ct);
        return Ok(result);
    }

    /// <summary>Desativa um usuário do sistema (soft delete). Requer papel Admin.</summary>
    /// <param name="id">ID do usuário a ser desativado.</param>
    /// <response code="204">Usuário desativado com sucesso.</response>
    /// <response code="401">Token de autenticação ausente ou inválido.</response>
    /// <response code="403">Usuário não possui permissão para acessar este recurso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _commands.DispatchAsync(new DeleteUserCommand(id), ct);
        return NoContent();
    }
}
