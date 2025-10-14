using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuroraScienceHub.Framework.Diagnostics.HealthChecks;

internal sealed class NonDuplicateHealthChecksBuilder : IHealthChecksBuilder
{
    private readonly IHealthChecksBuilder _innerBuilder;
    private static readonly HashSet<string> s_names = new();

    public NonDuplicateHealthChecksBuilder(IHealthChecksBuilder innerBuilder)
    {
        _innerBuilder = innerBuilder;
    }

    public IHealthChecksBuilder Add(HealthCheckRegistration registration)
    {
        if (s_names.Add(registration.Name))
        {
            _innerBuilder.Add(registration);
        }

        return this;
    }

    public IServiceCollection Services => _innerBuilder.Services;
}
