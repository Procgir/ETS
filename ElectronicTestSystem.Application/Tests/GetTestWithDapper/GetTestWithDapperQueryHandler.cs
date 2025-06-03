using System.Text.Json;
using Dapper;
using ElectronicTestSystem.Application.Abstractions.Data;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Tests.GetTestWithDapper;

internal sealed class GetTestWithDapperQueryHandler : IQueryHandler<GetTestWithDapperQuery, TestResponse>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public GetTestWithDapperQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<TestResponse>> Handle(GetTestWithDapperQuery request, CancellationToken cancellationToken)
    {
        const string getTestSql = """
          SELECT 1
          FROM users
          WHERE id = @UserId;

          SELECT
              id AS Id,
              subjectName AS Subject,
              theme AS Theme,
              createdAt AS CreatedAt,
              authorId as AuthorId
          FROM tests
          WHERE id = @TestId;

          SELECT
               id AS Id,
               body AS Body,
               trueAnswerNumber AS TrueAnswerNumber,
               answerOptions AS AnswersOptions
          FROM tests_questions
          WHERE test_id = @TestId;
          """;

        using var connection = _connectionFactory.CreateConnection();
        await using var multi = await connection.QueryMultipleAsync(
            getTestSql,
            new {request.TestId, request.UserId});

        var isUserExists = await multi.ReadFirstAsync<bool>();
        if (!isUserExists) return Result.Failure<TestResponse>(UserErrors.NotFound);

        var test = await multi.ReadSingleOrDefaultAsync<TestResponse>();
        if (test is null) return Result.Failure<TestResponse>(TestErrors.NotFound);

        if (test.AuthorId != request.UserId) return Result.Failure<TestResponse>(TestErrors.CantGetNotOwnTest);

        var questionsData = await multi.ReadAsync<(Guid Id, string Body,
            string AnswersOptions, int TrueAnswerNumber)>();

        test.Questions = questionsData
            .Select(q => new TestQuestionResponse
            {
                Id = q.Id,
                Body = q.Body,
                AnswersOptions = JsonSerializer.Deserialize<string[]>(q.AnswersOptions)!,
                TrueAnswerNumber = q.TrueAnswerNumber
            }).ToArray();

        return test;
    }
}