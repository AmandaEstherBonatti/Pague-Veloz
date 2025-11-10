using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

namespace PagueVeloz.Infrastructure.Resilience;

public static class RetryPolicyExtensions
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, _, retryCount, _) =>
                {
                    // Logging opcional pode ser adicionado aqui conforme necessário.
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker reset
                });
    }

    public static IAsyncPolicy<T> GetRetryPolicy<T>()
    {
        return Policy<T>
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, _, _, _) =>
                {
                    // Logging opcional pode ser adicionado aqui conforme necessário.
                });
    }
}

