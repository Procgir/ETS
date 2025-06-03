using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.SDK.Data;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Application.MarkCalculators;

public interface IMarkCalculatorFacade
{    
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Dictionary<string, GroupDto> GetAllGroups();
    public void SaveGroup(GroupDto groupDto);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Dictionary<string, TestDto> GetAllTests();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveTest(TestDto testDto);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<UserMarkDto> CalculateMarks(List<int> correctQuestionsAnswers, List<UserAnswersDto> usersAnswersDto);
    
    public Guid CreateTest(TestDto testDto);
    public TestDto GetTest(Guid testId);
    public PaginatedResult<TestDto> GetTests(GetOptions getOptions);
    public void DeleteTest(Guid testId);
    public void UpdateTest(Guid testId, UpdateTestDto testDto);
    
    
}