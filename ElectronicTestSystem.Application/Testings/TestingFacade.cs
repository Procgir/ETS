using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Testings;

public class TestingFacade : ITestingFacade
{
    private readonly IUserService _userService;
    private readonly ITestService _testService;
    private readonly IGroupService _groupService;
    private readonly ITestingService _testingService;
    private readonly ITestingUserAnswerService _testingUserAnswerService;
    private readonly IMarkCalculator _markCalculator;

    public TestingFacade(
        IUserService userService,
        ITestService testService,
        IGroupService groupService,
        ITestingService testingService, 
        ITestingUserAnswerService testingUserAnswerService, 
        IMarkCalculator markCalculator)
    {
        _userService = userService;
        _testService = testService;
        _groupService = groupService;
        _testingService = testingService;
        _testingUserAnswerService = testingUserAnswerService;
        _markCalculator = markCalculator;
    }
    
    public Guid CreateTesting(CreateTestingDto createTestingDto)
    {
        var test = _testingService.Get(createTestingDto.TestId);
        var group = _groupService.Get(createTestingDto.GroupId);
        
        var testing = Testing.Create(test.Id, group.Id, createTestingDto.EndedAt, createTestingDto.AuthorId);
        
        _testingService.Create(testing);
        
        return testing.Id;
    }

    public void UpdateTesting(Guid testingId, UpdateTestingDto updateTestingDto)
    {
        var testing = _testingService.Get(testingId);

        testing.ChangeTesting(updateTestingDto.EndedAt);
        
        _testingService.Update(testing);
    }

    public Guid CreateTestingAnswers(Guid testingId, Guid userId, CreateTestingAnswersDto createTestingAnswersDto)
    {
        var testing = _testingService.Get(testingId);
        if (testing.StatusAt(DateTime.Now) != TestingStatus.Active)
        {
            //TODO: add validations
            throw new Exception("Testing is finished!");
        }

        var user = _userService.Get(userId);
        var userGroup = _groupService.FindByUserId(user.Id);

        if (userGroup == null || 
            userGroup.Id != testing.GroupId)
        {
            //TODO: add validations
            throw new Exception("User is not in testing group!");
        }
        
        //TODO: extract logic to serviсe
        var answers = createTestingAnswersDto.Answers
            .Select(a => new UserAnswer(a.QuestionId, new TestQuestionAnswerNumber(a.AnswerNumber)))
            .ToArray();
        
        var userAnswers = TestingUserAnswers.Create(testing.Id, user.Id, answers, null);
        return _testingUserAnswerService.Create(userAnswers);
    }

    public void UpdateTestingAnswers(Guid testingId, Guid userId, UpdateTestingAnswersDto updateTestingAnswersDto)
    {
        var testing = _testingService.Get(testingId);
        if (testing.StatusAt(DateTime.Now) != TestingStatus.Active)
        {
            //TODO: add validation
            throw new Exception("Testing is finished!");
        }

        var user = _userService.Get(userId);
        var userGroup = _groupService.FindByUserId(user.Id);

        if (userGroup == null || 
            userGroup.Id != testing.GroupId)
        {
            //TODO: add validation
            throw new Exception("User is not in testing group!");
        }

        //TODO: extract logic to serviсe
        var answers = updateTestingAnswersDto.Answers
            .Select(a => new UserAnswer(a.QuestionId, new TestQuestionAnswerNumber(a.AnswerNumber)))
            .ToArray();

        var userAnswers = _testingUserAnswerService.GetByAsync(testing.Id, user.Id, default).Result;
        userAnswers.Change(answers);
        _testingUserAnswerService.Update(userAnswers);
    }
}