using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ElectronicTestSystem.WebApi.Healthchecks;

public abstract class CustomHealthCheckBase : IHealthCheck
{
    public abstract Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken());

    protected async Task<HealthCheckResult> WrapHealthCheck(string name, Func<CancellationToken, Task> check,
        CancellationToken cancellationToken)
    {
        try
        {
            await check(cancellationToken);

            return HealthCheckResult.Healthy(name);
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}