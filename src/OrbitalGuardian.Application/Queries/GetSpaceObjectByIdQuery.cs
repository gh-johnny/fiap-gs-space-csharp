using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public record GetSpaceObjectByIdQuery(Guid Id) : IQuery<SpaceObjectResponse>;
