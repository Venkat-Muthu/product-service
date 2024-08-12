namespace Buyz.Goodz.API.Extensions;

public static class HealthCheckExtension
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services.AddResourceMonitoring();
        services.AddHealthChecks()
            .AddResourceUtilizationHealthCheck();
        return services;
    }
}
