using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Application.Dto;

public class TestDto(string name, List<int> correctQuestionsAnswers)
{
    public string Name { get; } = name;
    public List<int> CorrectQuestionsAnswers { get; } = correctQuestionsAnswers;
    
    public static TestDto From(Test test) => new TestDto(test.Name, test.CorrectQuestionsAnswers);

    public static TestDto Empty() => new TestDto(string.Empty, new List<int>());

}