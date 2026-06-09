using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Commands;

public class DeleteSpaceObjectCommandHandler : ICommandHandler<DeleteSpaceObjectCommand, bool>
{
    private readonly ISpaceObjectRepository _repo;

    public DeleteSpaceObjectCommandHandler(ISpaceObjectRepository repo) => _repo = repo;

    public async Task<bool> HandleAsync(DeleteSpaceObjectCommand command, CancellationToken ct)
    {
        var spaceObject = await _repo.GetByIdAsync(command.SpaceObjectId, ct)
            ?? throw new SpaceObjectNotFoundException(command.SpaceObjectId.ToString());

        spaceObject.Deactivate();
        await _repo.UpdateAsync(spaceObject, ct);
        return true;
    }
}
