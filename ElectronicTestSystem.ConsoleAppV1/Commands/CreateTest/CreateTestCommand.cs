using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

namespace ElectronicTestSystem.ConsoleAppV1.Commands.CreateTest;

public class CreateTestCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
    : ConsoleCommand(app, ConsoleCommandNames.CreateTest)
{
    public override void Execute()
    {
        var testInfo = ReadTestInfo();
        
        SaveTest(testInfo.testName, testInfo.correctQuestionsAnswers);
    }

    private (string testName, List<int> correctQuestionsAnswers) ReadTestInfo()
    {
        Dictionary<string, TestDto> testNamesToTestsDict = markCalculatorFacade.GetAllTests();

        return (ReadTestName(testNamesToTestsDict), ReadTestCorrectQuestionsAnswers());
    }
    
    private void SaveTest(string testName, List<int> testCorrectQuestionsAnswers)
    {
        TestDto test = new TestDto(testName, testCorrectQuestionsAnswers);

        markCalculatorFacade.SaveTest(test);
    }
    
    private string ReadTestName(Dictionary<string, TestDto> testsCorrectQuestionsAnswers)
    {
        return App.ReadInputUntilValid(CreateTestMessages.EnterTestNameForSaving, (string input, out string errorMessage) =>
        {
            errorMessage = string.Empty;
            if (testsCorrectQuestionsAnswers.Count > 0 && testsCorrectQuestionsAnswers.ContainsKey(input))
            {
                errorMessage = CreateTestMessages.TestExistsWithSameName;
                return false;
            }
    
            return true;
        });
    }

    private List<int> ReadTestCorrectQuestionsAnswers()
    {
        Console.WriteLine(CreateTestMessages.EnterCorrectQuestionsAnswers);
        
        App.TryReadInputListUntilStopOrEnd(
            CreateTestMessages.EnterCorrectAnswerNumber,
            (input, count) => false,
            string.Empty,
            (string? input, out int parsedInput) => int.TryParse(input, out parsedInput),
            CreateTestMessages.IncorrectInputFormatNeedToRetryTemplate,
            string.Empty,
            out List<int> testQuestionsAnswers);
    
        return testQuestionsAnswers;
    }
}