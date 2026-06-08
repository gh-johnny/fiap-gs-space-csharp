using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserResponse>;
