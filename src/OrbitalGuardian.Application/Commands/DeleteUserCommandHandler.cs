using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Commands;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public DeleteUserCommandHandler(IUserRepository repo) => _repo = repo;

    public async Task<bool> HandleAsync(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(command.UserId, ct)
            ?? throw new UserNotFoundException(command.UserId.ToString());

        user.Deactivate();
        await _repo.UpdateAsync(user, ct);
        return true;
    }
}
