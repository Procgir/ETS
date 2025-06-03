using System.Data;

namespace ElectronicTestSystem.Application.Abstractions.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}