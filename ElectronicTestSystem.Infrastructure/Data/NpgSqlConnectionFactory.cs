using System.Data;
using ElectronicTestSystem.Application.Abstractions.Data;
using Npgsql;

namespace ElectronicTestSystem.Infrastructure.Data;

internal sealed class NpgSqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    
    public NpgSqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return connection;
    }
}