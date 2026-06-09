using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record DeleteConjunctionEventCommand(Guid ConjunctionEventId) : ICommand<bool>;
