using Microsoft.Extensions.DependencyInjection;
using OrbitalGuardian.Application.Interfaces;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Infrastructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    public async Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken ct)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetRequiredService(handlerType);

        OrbitalLogger.LogDebug("CommandDispatcher", $"Dispatching {command.GetType().Name}");

        var method = handlerType.GetMethod("HandleAsync")!;
        return await (Task<TResult>)method.Invoke(handler, [command, ct])!;
    }
}
