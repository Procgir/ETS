using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Exceptions;

namespace ElectronicTestSystem.Application.Testings.DeleteActiveTesting;

public class DeleteActiveTestingCommandHandler : ICommandHandler<DeleteActiveTestingCommand, TestingsItemResponse>
{
    private readonly ITestingService _testingService;
    private readonly IGroupService _groupService;
    private readonly ITestService _testService;
    private readonly ITestingRepository _testingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteActiveTestingCommandHandler(
        ITestingService testingService,
        IGroupService groupService,
        ITestService testService,
        ITestingRepository testingRepository,
        IUnitOfWork unitOfWork)
    {
        _testingService = testingService;
        _groupService = groupService;
        _testService = testService;
        _testingRepository = testingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TestingsItemResponse>> Handle(DeleteActiveTestingCommand request, CancellationToken cancellationToken)
    {
        var testing = await _testingService.GetAsync(request.TestingId, cancellationToken);

        if (testing == null)
        {
            return Result.Failure<TestingsItemResponse>(TestingErrors.NotFound);
        }

        if (testing.StatusAt(DateTime.Now) != TestingStatus.Active)
        {
            return Result.Failure<TestingsItemResponse>(TestingErrors.CantCreateAnswersTestingWasFinished);
        }

        var group = _groupService.Get(testing.GroupId);
        var test = _testService.Get(testing.TestId);

        var response = new TestingsItemResponse(
            testing.Id,
            TestingsItemResponseStatus.Active,
            group != null && group.Name != null ? group.Name.Value : string.Empty,
            test != null && test.Theme != null ? test.Theme.Name : string.Empty,
            testing.CreatedAt,
            testing.EndedAt,
            0,
            0
        );

        try
        {
            _testingRepository.Delete(testing.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<TestingsItemResponse>(TestingErrors.CantCreateAnswersItIsJustChanged);
        }

        return Result.Success(response);
    }
}