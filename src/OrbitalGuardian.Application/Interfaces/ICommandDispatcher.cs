namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Dispatches commands to their registered handlers via reflection.</summary>
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken ct);
}
