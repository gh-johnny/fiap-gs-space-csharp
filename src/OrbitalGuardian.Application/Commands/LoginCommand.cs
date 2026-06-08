using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record LoginCommand(string Email, string Password) : ICommand<AuthTokenResponse>;
