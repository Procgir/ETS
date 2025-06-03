using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using MediatR.NotificationPublishers;

namespace ElectronicTestSystem.Application.Testings.GetTesting;

internal sealed class GetTestingQueryHandler : IQueryHandler<GetTestingQuery, TestingResponse>
{
    private readonly ITestingRepository _testingRepository;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;
    private readonly ITestRepository _testRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupService _groupService;
    private readonly IMarkCalculator _markCalculator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetTestingQueryHandler(ITestRepository testRepository, IUserRepository userRepository, ITestingRepository testingRepository, IDateTimeProvider dateTimeProvider, IGroupService groupService, ITestingUserAnswersRepository testingUserAnswersRepository, IMarkCalculator markCalculator)
    {
        _testRepository = testRepository;
        _userRepository = userRepository;
        _testingRepository = testingRepository;
        _dateTimeProvider = dateTimeProvider;
        _groupService = groupService;
        _testingUserAnswersRepository = testingUserAnswersRepository;
        _markCalculator = markCalculator;
    }

    public async Task<Result<TestingResponse>> Handle(GetTestingQuery request, CancellationToken cancellationToken)
    {
        var testing = await _testingRepository.GetAsync(request.TestingId, cancellationToken);

        if (testing is null)
        {
            return Result.Failure<TestingResponse>(TestingErrors.NotFound);
        }
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<TestingResponse>(UserErrors.NotFound);
        }

        if (testing.AuthorId != user.Id)
        {
            return Result.Failure<TestingResponse>(TestingErrors.CantGetNotOwnTesting);
        }

        var test = await _testRepository.GetByIdAsync(testing.TestId, cancellationToken);

        if (test is null)
        {
            return Result.Failure<TestingResponse>(TestErrors.NotFound);
        }

        var group = _groupService.Get(testing.GroupId);
        var usersAnswers = await _testingUserAnswersRepository.GetByAsync(testing.Id, cancellationToken);

        var usersResults = _markCalculator.CalculateMarks(
            test.Questions,
            usersAnswers);

        //TODO: add Response Builder for difficult cases like this
        var allUserResults = group
            .Users
            .Select(u =>
            {
                var userResult = usersResults.FirstOrDefault(ur => ur.UserId == u.Id);
                return userResult ?? TestingResult.Empty(u.Id);
            })
            .ToArray();

        var testingResponse = new TestingResponse(
            group.Name.Value,
            $"{test.Subject.Name} {test.Theme.Name}", 
            testing.CreatedAt,
            testing.EndedAt,
            usersAnswers.Length,
            group.Users.Count,
            allUserResults.Select(ur => ur.ToResponse(group.Users.First(u => u.Id == ur.UserId))).ToArray());
        
        return testingResponse;
    }
}