namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Handler for a CQRS command of type TCommand producing TResult.</summary>
public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
}
