using AuroraScienceHub.Framework.Configuration;
using AuroraScienceHub.Framework.Utilities.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;

namespace AuroraScienceHub.Framework.Logging.OpenTelemetry;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var hostOptions = GetHostOptions(configuration);

        builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddApplicationName(hostOptions.ResolveHostName())
                    .AddNamespace(hostOptions.ResolveHostNamespace()));
        });

        builder.SetMinimumLevel(LogLevel.Information);

        return builder;
    }

    private static HostOptions GetHostOptions(IConfiguration configuration) =>
        configuration
            .GetRequiredOptions<HostOptions>()
        ?? throw new ArgumentException($"Configuration section «{HostOptions.OptionKey}» not found");
}
