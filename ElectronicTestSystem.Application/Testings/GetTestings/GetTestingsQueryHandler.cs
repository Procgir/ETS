using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.Testings.GetTesting;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Testings.GetTestings;

internal sealed class GetTestingsQueryHandler : IQueryHandler<GetTestingsQuery, PagedResultResponse<TestingsItemResponse>>
{
    private readonly ITestingRepository _testingRepository;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;
    private readonly ITestRepository _testRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupService _groupService;
    private readonly IMarkCalculator _markCalculator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetTestingsQueryHandler(ITestRepository testRepository, IUserRepository userRepository, ITestingRepository testingRepository, IDateTimeProvider dateTimeProvider, IGroupService groupService, ITestingUserAnswersRepository testingUserAnswersRepository, IMarkCalculator markCalculator)
    {
        _testRepository = testRepository;
        _userRepository = userRepository;
        _testingRepository = testingRepository;
        _dateTimeProvider = dateTimeProvider;
        _groupService = groupService;
        _testingUserAnswersRepository = testingUserAnswersRepository;
        _markCalculator = markCalculator;
    }

    public async Task<Result<PagedResultResponse<TestingsItemResponse>>> Handle(GetTestingsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<PagedResultResponse<TestingsItemResponse>>(UserErrors.NotFound);
        }

        var pagedTestings = await _testingRepository.GetPagedByAuthorAsync(request.UserId, request.Options, cancellationToken);
        var testsIds = pagedTestings.Items.Select(t => t.TestId).ToArray();
        var tests = await _testRepository.GetByAsync(testsIds, cancellationToken);

        var groupsIds = pagedTestings
            .Items
            .Select(t => t.GroupId)
            .Distinct()
            .ToArray();
        
        var groupsDictionary = (await _groupService.GetByAsync(groupsIds, cancellationToken))
            .ToDictionary(g => g.Id);

        var groupIdDoneCountDict = new Dictionary<Guid, int>(groupsIds.Length);

        foreach (var testing in pagedTestings.Items)
        {
            var userAnswers = await _testingUserAnswersRepository.GetByAsync(testing.Id, cancellationToken);
            
            groupIdDoneCountDict.Add(testing.GroupId, userAnswers.Length);
        }
        
        return DtoBuilderHelper.Build(pagedTestings, tests, groupsDictionary, groupIdDoneCountDict, _dateTimeProvider.Now);
    }
}