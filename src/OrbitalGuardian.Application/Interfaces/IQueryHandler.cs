namespace OrbitalGuardian.Application.Interfaces;

/// <summary>Handler for a CQRS query of type TQuery returning TResult.</summary>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
}
