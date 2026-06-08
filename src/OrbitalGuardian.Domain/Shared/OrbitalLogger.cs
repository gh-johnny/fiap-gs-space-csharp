using Microsoft.Extensions.Logging;

namespace OrbitalGuardian.Domain.Shared;

public static class OrbitalLogger
{
    private static ILogger? _logger;

    public static void Initialize(ILoggerFactory loggerFactory) =>
        _logger = loggerFactory.CreateLogger("OrbitalGuardian");

    public static void LogInfo(string category, string message) =>
        _logger?.LogInformation("[{Category}] {Message}", category, message);

    public static void LogWarning(string category, string message) =>
        _logger?.LogWarning("[{Category}] {Message}", category, message);

    public static void LogError(string category, string message, Exception? ex = null) =>
        _logger?.LogError(ex, "[{Category}] {Message}", category, message);

    public static void LogDebug(string category, string message) =>
        _logger?.LogDebug("[{Category}] {Message}", category, message);
}
