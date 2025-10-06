using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace DimDim.OrdersApi.Observability;

public static class Telemetry
{
    public static IServiceCollection AddDimDimTelemetry(this IServiceCollection services, IConfiguration config)
    {
        // Variável de ambiente: APPLICATIONINSIGHTS_CONNECTION_STRING
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName: "DimDim.OrdersApi"))
            .UseAzureMonitor();

        return services;
    }
}
