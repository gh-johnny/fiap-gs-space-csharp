using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Aggregates.Users;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Application.Commands;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, UserResponse>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;

    public RegisterCommandHandler(IUserRepository repo, IPasswordHasher hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task<UserResponse> HandleAsync(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _repo.GetByEmailAsync(command.Email, ct);
        if (existing is not null)
            throw new DuplicateEmailException(command.Email);

        var passwordHash = _hasher.Hash(command.Password);
        var user = User.Create(command.Email, passwordHash, command.FullName, command.Role);

        await _repo.AddAsync(user, ct);

        OrbitalLogger.LogInfo("Register", $"User registered: {user.Email}");

        return new UserResponse
        {
            Id = user.Id, Email = user.Email, FullName = user.FullName,
            Role = user.Role, CreatedAt = user.CreatedAt, IsActive = user.IsActive
        };
    }
}
