using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace OrbitalGuardian.Infrastructure.Repositories;

internal static class RepositoryPolicy
{
    public static AsyncRetryPolicy CreateRetryPolicy(int retries = 3) =>
        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retries, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)));

    public static AsyncTimeoutPolicy CreateTimeoutPolicy(int seconds = 10) =>
        Policy.TimeoutAsync(seconds, TimeoutStrategy.Optimistic);
}
