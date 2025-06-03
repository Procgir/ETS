using Dapper;
using ElectronicTestSystem.Application.Abstractions.Data;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Application.Tests.FindTest;

internal sealed class FindTestQueryHandler : IQueryHandler<FindTestQuery, TestsResponse>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public FindTestQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<TestsResponse>> Handle(FindTestQuery request, CancellationToken cancellationToken)
    {
        const string getTestSql = """
          SELECT
              id AS Id,
              subjectName AS Subject,
              theme AS Theme,
              createdAt AS CreatedAt
          FROM tests
          WHERE authorId = @UserId 
            and (subjectName like '%' + @Query + '%' or theme like '%' + @Query + '%');
          """;

        using var connection = _connectionFactory.CreateConnection();
        var tests = await connection.QueryAsync<TestShortResponse>(
            getTestSql,
            new {request.Query, request.UserId});


        return new TestsResponse
        {
            Tests = tests?.ToArray() ?? Array.Empty<TestShortResponse>()
        };
    }
}