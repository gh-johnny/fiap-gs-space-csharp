using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Commands;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthTokenResponse>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwtService;

    public LoginCommandHandler(IUserRepository repo, IPasswordHasher hasher, IJwtTokenService jwtService)
    {
        _repo = repo;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    public async Task<AuthTokenResponse> HandleAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await _repo.GetByEmailAsync(command.Email, ct)
            ?? throw new InvalidCredentialsException();

        if (!_hasher.Verify(command.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = _jwtService.GenerateToken(user);

        OrbitalLogger.LogInfo("Login", $"User logged in: {user.Email}");

        return new AuthTokenResponse
        {
            Token = token, Email = user.Email, FullName = user.FullName,
            Role = user.Role, ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}
