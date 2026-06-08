using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;
using OrbitalGuardian.Domain.ValueObjects;

namespace OrbitalGuardian.Application.Commands;

public class AddTelemetryCommandHandler : ICommandHandler<AddTelemetryCommand, TelemetryReadingResponse>
{
    private readonly ISpaceObjectRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public AddTelemetryCommandHandler(ISpaceObjectRepository repo, IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task<TelemetryReadingResponse> HandleAsync(AddTelemetryCommand command, CancellationToken ct)
    {
        var obj = await _repo.GetByIdAsync(command.SpaceObjectId, ct)
            ?? throw new SpaceObjectNotFoundException(command.SpaceObjectId.ToString());

        var position = Coordinates.Create(command.X, command.Y, command.Z);
        var velocity = Coordinates.Create(command.Vx, command.Vy, command.Vz);
        var stateVector = StateVector.Create(position, velocity, command.UncertaintyKm);

        obj.RecordTelemetry(stateVector, command.Timestamp);

        await _repo.UpdateAsync(obj, ct);
        await _dispatcher.DispatchAsync(obj.GetDomainEvents(), ct);
        obj.ClearDomainEvents();

        OrbitalLogger.LogInfo("AddTelemetry", $"Telemetry recorded for object {obj.NoradId}");

        var latest = obj.TelemetryReadings.Latest()!;
        return new TelemetryReadingResponse
        {
            Id = latest.Id, SpaceObjectId = latest.SpaceObjectId, Timestamp = latest.Timestamp,
            X = latest.StateVector.Position.X, Y = latest.StateVector.Position.Y, Z = latest.StateVector.Position.Z,
            Vx = latest.StateVector.Velocity.X, Vy = latest.StateVector.Velocity.Y, Vz = latest.StateVector.Velocity.Z,
            UncertaintyKm = latest.StateVector.PositionalUncertaintyKm
        };
    }
}
