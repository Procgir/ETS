using ElectronicTestSystem.Application.Testings.GetTesting;
using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Application.Tests.GetTests;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.Dto;

public static class DtoBuilderExtensions
{
    public static TestResponse ToResponse(this Test test) 
        => new (test.Id, 
            test.Subject.Name,
            test.Theme.Name,
            test.CreatedAt,
            test.Questions
                .Select(tq => tq.ToResponse())
                .ToArray(),
            test.AuthorId);

    public static TestShortResponse ToResponseShort(this Test test)
        => new(test.Id,
            test.Subject.Name,
            test.Theme.Name,
            test.CreatedAt);
    
    public static TestQuestionResponse ToResponse(this TestQuestion question) 
        => new (
            question.Id, 
            question.Body.Text, 
            question.Answers
                .OrderBy(a => a.Number)
                .Select(a => a.Text)
                .ToArray(), 
            question.TrueAnswerNumber.Value);

    public static TestingUserResultResponse ToResponse(this TestingResult result, User user)
        => new (
            $"{user.Name.LastName} {user.Name.FirstName} {user.Name.MiddleName}", 
            result.TotalAnswersCount,
            result.IncorrectAnswersCount,
            result.CorrectAnswersCount,
            (int)result.Mark.Value);
    
    public static PagedResultResponse<TOut> ToResponse<TIn, TOut>(this PagedResult<TIn> result,
        Func<TIn, TOut> converter) 
        where TIn : class 
        where TOut :class
        => new (result.Result.Select(converter).ToArray(), result.PagerInfo.ToResponse());
    
    public static PagedResultResponse<TOut> ToResponse<TIn, TOut>(this PaginatedResult<TIn> result,
        Func<TIn, TOut> converter) 
        where TIn : class 
        where TOut :class
        => new (result.Items.Select(converter).ToArray(), new PagedResultResponsePagerInfo(result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages));

    public static PagedResultResponsePagerInfo ToResponse(this PagerInfo pager)
        => new (pager.PageNumber, pager.PageSize, pager.TotalCount ,pager.TotalPages);

    public static TestingsItemResponseStatus ToResponse(this TestingStatus status)
        => status switch
        {
            TestingStatus.Active => TestingsItemResponseStatus.Active,
            TestingStatus.Finished => TestingsItemResponseStatus.Finished,
            _ => TestingsItemResponseStatus.Unknown
        };
}