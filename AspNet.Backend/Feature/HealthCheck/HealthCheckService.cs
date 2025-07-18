using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNet.Backend.Feature.HealthCheck;

/// <summary>
/// The <see cref="HealthCheckService"/> class
/// is used by ASP.NET to indicate whether the backend is up and running or not.
/// </summary>
public sealed class HealthCheckService : IHealthCheck
{
    /// <summary>
    /// Checks the health state of the server async.
    /// </summary>
    /// <param name="context">The <see cref="HealthCheckContext"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> with the result.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // All is well!
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}