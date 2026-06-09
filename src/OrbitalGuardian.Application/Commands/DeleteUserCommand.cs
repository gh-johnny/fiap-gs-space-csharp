using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record DeleteUserCommand(Guid UserId) : ICommand<bool>;
