namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Dispatches queries to their registered handlers via reflection.</summary>
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken ct);
}
