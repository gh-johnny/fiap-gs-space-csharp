using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record DeleteSpaceObjectCommand(Guid SpaceObjectId) : ICommand<bool>;
