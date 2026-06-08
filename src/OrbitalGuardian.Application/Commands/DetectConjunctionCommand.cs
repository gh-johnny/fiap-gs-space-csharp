using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record DetectConjunctionCommand(
    Guid PrimaryObjectId,
    Guid SecondaryObjectId,
    DateTime PredictedTcaUtc
) : ICommand<ConjunctionEventResponse>;
