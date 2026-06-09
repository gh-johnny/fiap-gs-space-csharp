using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.Application.Commands;
using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _commands;

    public AuthController(ICommandDispatcher commands) => _commands = commands;

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new RegisterCommand(request.Email, request.Password, request.FullName, request.Role), ct);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }
}
