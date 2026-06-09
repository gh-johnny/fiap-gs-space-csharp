using Microsoft.Extensions.DependencyInjection;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Infrastructure.Dispatchers;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken ct)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        OrbitalLogger.LogDebug("QueryDispatcher", $"Dispatching {query.GetType().Name}");

        var method = handlerType.GetMethod("HandleAsync")!;
        return await (Task<TResult>)method.Invoke(handler, [query, ct])!;
    }
}
