using Microsoft.Extensions.DependencyInjection;

namespace PagueVeloz.Infrastructure.Middleware;

public static class RateLimitingMiddleware
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        return services;
    }
}

