using Microsoft.AspNetCore.Mvc;
using OrbitalGuardian.API.Swagger;
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

    /// <summary>Registra um novo usuário no sistema. Acesso público.</summary>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="409">Já existe um usuário com o e-mail informado.</response>
    [SwaggerBodyExample("""
        {
          "email": "operator@orbitalguardian.com",
          "password": "Operator@123",
          "fullName": "Carlos Operador",
          "role": 1
        }
        """)]
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new RegisterCommand(request.Email, request.Password, request.FullName, request.Role), ct);
        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>Autentica um usuário e retorna um token JWT. Acesso público.</summary>
    /// <response code="200">Autenticação realizada com sucesso. Retorna o token JWT.</response>
    /// <response code="400">Dados inválidos na requisição.</response>
    /// <response code="401">Credenciais incorretas.</response>
    [SwaggerBodyExample("""
        {
          "email": "admin@orbitalguardian.com",
          "password": "Admin@123"
        }
        """)]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokenResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _commands.DispatchAsync(
            new LoginCommand(request.Email, request.Password), ct);
        return Ok(result);
    }
}
