using Microsoft.Extensions.DependencyInjection;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Infrastructure.Dispatchers;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    public async Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            OrbitalLogger.LogDebug("DomainEventDispatcher", $"Dispatching {domainEvent.GetType().Name}");

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync")!;
                await (Task)method.Invoke(handler, [domainEvent, ct])!;
            }
        }
    }
}
