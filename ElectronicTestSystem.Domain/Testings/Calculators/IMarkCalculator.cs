using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Domain.Testings.Calculators;

public interface IMarkCalculator
{
    public Mark CalculateMark(List<int> testsCorrectAnswers, List<int> userAnswersIndexes);

    public List<(string UserName, Mark Mark)> CalculateMarks(List<int> testsCorrectAnswers,
        List<TestingUserAnswers> usersAnswers);
    
    public TestingResult[] CalculateMarks(IReadOnlyCollection<TestQuestion> questions,
        TestingUserAnswers[] usersAnswers);
}