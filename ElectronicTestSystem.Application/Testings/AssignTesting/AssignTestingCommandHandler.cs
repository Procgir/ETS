using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Testings.AssignTesting;

internal sealed class AssignTestingCommandHandler : ICommandHandler<AssignTestingCommand, AssignTestingResponse>
{
    private readonly ITestingRepository _testingRepository;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;
    private readonly ITestingService _testingService;
    private readonly IUserRepository _userRepository;
    private readonly IGroupService _groupService;
    private readonly ITestService _testService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AssignTestingCommandHandler(ITestingService testingService, IUserRepository userRepository, ITestingRepository testingRepository, IDateTimeProvider dateTimeProvider, IGroupService groupService, ITestingUserAnswersRepository testingUserAnswersRepository,
        ITestService testService)
    {
        _testingService = testingService;
        _userRepository = userRepository;
        _testingRepository = testingRepository;
        _dateTimeProvider = dateTimeProvider;
        _groupService = groupService;
        _testingUserAnswersRepository = testingUserAnswersRepository;
        _testService = testService;
    }

    public async Task<Result<AssignTestingResponse>> Handle(AssignTestingCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AssignTestingResponse>(UserErrors.NotFound);
        }

        if (!user.IsTeacher)
        {
            return Result.Failure<AssignTestingResponse>(TestingErrors.CantAssignTestingUserMustBeTeacher);
        }

        var test = await _testingService.GetAsync(request.TestId, cancellationToken);

        if (test is null)
        {
            return Result.Failure<AssignTestingResponse>(TestErrors.NotFound);
        }

        if (test.AuthorId != user.Id)
        {
            return Result.Failure<AssignTestingResponse>(TestingErrors.CantAssignTestingUserMustBeAnOwner);
        }

        var group = await _groupService.GetByAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Result.Failure<AssignTestingResponse>(GroupErrors.NotFound);
        }

        var testing = Testing.Create(test.Id, group.Id, request.EndDate, user.Id);

        _testingService.Create(testing);

        return new AssignTestingResponse(testing.Id);
    }
}