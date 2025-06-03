using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.Dto;

public static class DtoBuilderHelper
{
    public static PagedResultResponse<TestingsItemResponse> Build(
        PaginatedResult<Testing> pagedTestings,
        Test[] tests,
        IReadOnlyDictionary<Guid, Group> groupsDict,
        IReadOnlyDictionary<Guid, int> groupIdDoneCountDict,
        DateTime now)
    {
        var testingsWithTestsPairs = pagedTestings
            .Items
            .Join(tests,
                t => t.TestId,
                test => test.Id,
                (testing, test) => new {Testing = testing, Test = test});

        var items = testingsWithTestsPairs
            .Select(t =>
            {
                var testing = t.Testing;
                var test = t.Test;
                var groupName = groupsDict[testing.GroupId].Name.Value;
                var testName = $"{test.Subject.Name} {t.Test.Theme.Name}";

                return new TestingsItemResponse(
                    testing.Id,
                    testing.StatusAt(now).ToResponse(),
                    groupName,
                    testName,
                    testing.CreatedAt,
                    testing.EndedAt,
                    groupIdDoneCountDict[testing.GroupId],
                    groupsDict[testing.GroupId].Users.Count);
            })
            .ToArray();

        return new(
            items,
            new(pagedTestings.PageNumber, pagedTestings.PageSize, pagedTestings.TotalCount, pagedTestings.TotalPages));
    }
}