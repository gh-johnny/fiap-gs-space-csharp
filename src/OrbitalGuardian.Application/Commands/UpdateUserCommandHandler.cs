using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Commands;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _repo;

    public UpdateUserCommandHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserResponse> HandleAsync(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(command.UserId, ct)
            ?? throw new UserNotFoundException(command.UserId.ToString());

        user.UpdateProfile(command.FullName);
        await _repo.UpdateAsync(user, ct);

        return new UserResponse
        {
            Id = user.Id, Email = user.Email, FullName = user.FullName,
            Role = user.Role, CreatedAt = user.CreatedAt, IsActive = user.IsActive
        };
    }
}
