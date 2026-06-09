using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;

namespace OrbitalGuardian.Application.Commands;

public class DeleteConjunctionEventCommandHandler : ICommandHandler<DeleteConjunctionEventCommand, bool>
{
    private readonly IConjunctionEventRepository _repo;

    public DeleteConjunctionEventCommandHandler(IConjunctionEventRepository repo) => _repo = repo;

    public async Task<bool> HandleAsync(DeleteConjunctionEventCommand command, CancellationToken ct)
    {
        var conjunction = await _repo.GetByIdAsync(command.ConjunctionEventId, ct)
            ?? throw new ConjunctionEventNotFoundException(command.ConjunctionEventId.ToString());

        await _repo.DeleteAsync(command.ConjunctionEventId, ct);
        return true;
    }
}
