using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Enums;

namespace OrbitalGuardian.Application.Commands;

public record RegisterCommand(string Email, string Password, string FullName, UserRole Role) : ICommand<UserResponse>;
