using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Queries;

public record GetActiveConjunctionsQuery : IQuery<IReadOnlyList<ConjunctionEventResponse>>;
