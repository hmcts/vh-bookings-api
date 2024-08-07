using BookingsApi.DAL;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookingsApi.Health;

public static class HealthCheckExtensions
{
    private static readonly string[] HealthCheckTags = {"startup", "readiness"};
    public static IServiceCollection AddVhHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusSettings = new ServiceBusSettings();
        configuration.GetSection("ServiceBusQueue").Bind(serviceBusSettings);
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDbContextCheck<BookingsDbContext>(name: "Database VhBookings", tags: HealthCheckTags)
            .AddAzureServiceBusQueue(serviceBusSettings!.ConnectionString, 
                serviceBusSettings.QueueName,
                name: "Booking Service Bus Queue", 
                tags: HealthCheckTags,
                failureStatus: HealthStatus.Degraded);
            
        return services;
    }
}
