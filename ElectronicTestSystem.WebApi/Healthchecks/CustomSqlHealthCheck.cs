using Dapper;
using ElectronicTestSystem.Application.Abstractions.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ElectronicTestSystem.WebApi.Healthchecks;

public class CustomSqlHealthCheck : IHealthCheck
{
    private readonly ISqlConnectionFactory _connectionFactory;
    
    public CustomSqlHealthCheck(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteScalarAsync("select 1;", cancellationToken);
            
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}