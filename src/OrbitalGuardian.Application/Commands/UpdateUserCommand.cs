using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record UpdateUserCommand(Guid UserId, string FullName) : ICommand<UserResponse>;
