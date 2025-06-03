using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Exceptions;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.Application.Testings.CreateTestingAnswers;

internal sealed class CreateTestingAnswersCommandHandler : ICommandHandler<CreateTestingAnswersCommand>
{
    private readonly ITestingRepository _testingRepository;
    private readonly ITestService _testService;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepository;
    private readonly ITestingService _testingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IGroupService _groupService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTestingAnswersCommandHandler(ITestingService testingService, IUserRepository userRepository, ITestingRepository testingRepository, IDateTimeProvider dateTimeProvider, IGroupService groupService, ITestingUserAnswersRepository testingUserAnswersRepository, ITestService testService, ITestingUserAnswerService testingUserAnswerService, IUnitOfWork unitOfWork)
    {
        _testingService = testingService;
        _userRepository = userRepository;
        _testingRepository = testingRepository;
        _dateTimeProvider = dateTimeProvider;
        _groupService = groupService;
        _testingUserAnswersRepository = testingUserAnswersRepository;
        _testService = testService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateTestingAnswersCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound);
        }

        var testing = await _testingRepository.GetAsync(request.TestingId, cancellationToken);

        if (testing is null)
        {
            return Result.Failure(TestingErrors.NotFound);
        }

        if (testing.StatusAt(_dateTimeProvider.Now) == TestingStatus.Finished)
        {
            return Result.Failure(TestingErrors.CantCreateAnswersTestingWasFinished);
        }
        
        var userGroup = _groupService.FindByUserId(request.UserId);
        if (userGroup is null)
        {
            return Result.Failure(GroupErrors.NotFound);
        }

        if (testing.GroupId != userGroup.Id)
        {
            return Result.Failure(TestingErrors.CantCreateAnswersUserIsNotInTestingGroup);
        }

        var test = _testService.Get(testing.TestId);
        if (test.Questions.Count != request.UserAnswers.Length)
        {
            return Result.Failure(TestingErrors.CantCreateAnswersAnswersCountIsNotTheSameAsQuestionsCount);
        }

        var questionsIdsOrdered = test
            .Questions
            .Select(q => q.Id)
            .OrderBy(qid => qid)
            .ToArray();

        var userAnswers = request
            .UserAnswers
            .WithIndex()
            .Select(ua =>
            {
                var questionId = questionsIdsOrdered[ua.Index];
                return new UserAnswer(questionId, new TestQuestionAnswerNumber(ua.Item));
            })
            .ToArray();
        
        var testingUserAnswers = TestingUserAnswers.Create(testing.Id, user.Id, userAnswers, null);

        try
        {
            _testingUserAnswersRepository.Add(testingUserAnswers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException e)
        {
            Result.Failure(TestingErrors.CantCreateAnswersItIsJustChanged);
        }    
        return Result.Success();
    }
}