using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public record GetUsersQuery : IQuery<IReadOnlyList<UserResponse>>;
