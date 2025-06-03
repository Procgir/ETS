using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.Testings.SearchTestings;

internal sealed class SearchTestingsQueryHandler : IQueryHandler<SearchTestingsQuery, PagedResultResponse<TestingsItemResponse>>
{
    private readonly ITestingRepository _testingRepository;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;
    private readonly ITestRepository _testRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupService _groupService;
    private readonly IMarkCalculator _markCalculator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SearchTestingsQueryHandler(
        ITestRepository testRepository,
        IUserRepository userRepository,
        ITestingRepository testingRepository,
        IDateTimeProvider dateTimeProvider,
        IGroupService groupService,
        ITestingUserAnswersRepository testingUserAnswersRepository,
        IMarkCalculator markCalculator)
    {
        _testRepository = testRepository;
        _userRepository = userRepository;
        _testingRepository = testingRepository;
        _dateTimeProvider = dateTimeProvider;
        _groupService = groupService;
        _testingUserAnswersRepository = testingUserAnswersRepository;
        _markCalculator = markCalculator;
    }

    public async Task<Result<PagedResultResponse<TestingsItemResponse>>> Handle(SearchTestingsQuery request, CancellationToken cancellationToken)
    {
        var testings = _testingRepository.Find(request.Query) ?? Array.Empty<Testing>();
        var testsIds = testings.Select(t => t.TestId).ToArray();
        var tests = await _testRepository.GetByAsync(testsIds, cancellationToken);

        var groupsIds = testings
            .Select(t => t.GroupId)
            .Distinct()
            .ToArray();

        var groupsDictionary = (await _groupService.GetByAsync(groupsIds, cancellationToken))
            .ToDictionary(g => g.Id);

        var groupIdDoneCountDict = new Dictionary<Guid, int>(groupsIds.Length);

        foreach (var testing in testings)
        {
            var userAnswers = await _testingUserAnswersRepository.GetByAsync(testing.Id, cancellationToken);
            groupIdDoneCountDict[testing.GroupId] = userAnswers.Length;
        }

        var pagedResult = new PaginatedResult<Testing>(testings, 1, testings.Length, testings.Length);
        var response = DtoBuilderHelper.Build(pagedResult, tests, groupsDictionary, groupIdDoneCountDict, _dateTimeProvider.Now);
        return Result.Success(response);
    }
}