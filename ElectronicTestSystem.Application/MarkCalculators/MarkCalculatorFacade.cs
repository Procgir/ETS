using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.MarkCalculators;

public class MarkCalculatorFacade(
    IGroupService groupService, 
    ITestService testService, 
    IMarkCalculator calculator)
    : IMarkCalculatorFacade
{
    public Dictionary<string, GroupDto> GetAllGroups()
    {
        var groups = groupService.GetAllGroups();

        return groups.ToDictionary(g => g.Name.Value, GroupDto.From);
    }

    public void SaveGroup(GroupDto groupDto)
    {
        groupService.SaveGroup(Group.Create(groupDto.Name, 
            groupDto.UsersNames.Select(User.Create).ToList()));
    }

    public Dictionary<string, TestDto> GetAllTests()
    {
        var tests = testService.GetAll();

        return tests.ToDictionary(t => t.Name, 
            t => new TestDto(t.Name, t.CorrectQuestionsAnswers));
    }

    public void SaveTest(TestDto testDto)
    {
        testService.SaveTest(Test.Create(testDto.Name, testDto.CorrectQuestionsAnswers));
    }

    public Guid CreateTest(TestDto testDto)
    {
        var test = Test.Create(testDto.Name, testDto.CorrectQuestionsAnswers);
        
        testService.Create(test);

        return test.Id;
    }

    public TestDto GetTest(Guid testId)
    {
        var test = testService.Get(testId);

        return TestDto.From(test);
    }

    public PaginatedResult<TestDto> GetTests(GetOptions getOptions)
    {
        var result = testService.Get(getOptions);
        
        return new PaginatedResult<TestDto>(
            result.Items.Select(TestDto.From), 
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }

    public void DeleteTest(Guid testId)
    {
        testService.Delete(testId);
    }

    public void UpdateTest(Guid testId, UpdateTestDto updateTestDto)
    {
        var subject = testService.CreateSubject(updateTestDto.Subject);
        var theme = testService.CreateTheme(updateTestDto.Theme);
        var questions = updateTestDto
            .Questions?
            .Select(q => testService.CreateQuestion(testId, q.Body, q.Answers, q.TrueAnswerNumber))
            .ToArray();
            
        var test = testService.Get(testId);
        test.Update(subject, theme, questions);
        
        testService.Update(test);
    }

    public List<UserMarkDto> CalculateMarks(List<int> correctQuestionsAnswers, List<UserAnswersDto> usersAnswersDto)
    {
        var marks = calculator.CalculateMarks(
            correctQuestionsAnswers, 
            usersAnswersDto.Select(u => TestingUserAnswers.Create(u.UserName, u.Answers)).ToList()); 
        
        return marks.Select(m => new UserMarkDto(m.UserName, m.Mark)).ToList();        
    }

  
}

