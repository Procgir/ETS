using ElectronicTestSystem.ConsoleApi.Clients.Abstractions.Tests;
using ElectronicTestSystem.ConsoleApi.Clients.Tests;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Results;

namespace ElectronicTestSystem.ConsoleAppV3.Commands.CreateTest;

public class CreateTestCommand(
    ConsoleCommandApp app,
     ITestsHttpClientApi testsHttpClientApi)
    : ConsoleCommand(app, ConsoleCommandNames.CreateTest)
{
    protected override void InitializeStages()
    {
        AddStage(CreateTestMessages.SingleStageName, SingleStageDoAsync);
    }

    private Result<ConsoleCommandsStageResult> SingleStageDoAsync()
    {
        var testInfo = ReadTestInfo();
        
        SaveTest(testInfo.testName, testInfo.correctQuestionsAnswers);

        var stageResult = GetStageResult(testInfo);
        
        return Result<ConsoleCommandsStageResult>.Success(stageResult);
    }

    private ConsoleCommandsStageResult GetStageResult((string testName, List<int> correctQuestionsAnswers) testInfo)
    {
        var inputStrings = new List<string>(testInfo.correctQuestionsAnswers.Count + 2);
 
        inputStrings.Add(string.Format(CreateTestMessages.TestNameInputTemplate, testInfo.testName));
        inputStrings.Add(string.Format(CreateTestMessages.CorrectQuestionAnswersTitleInput));

        foreach (var correctQuestionsAnswerIndexed in testInfo.correctQuestionsAnswers.WithIndex())
        {
            inputStrings.Add(string.Format(CreateTestMessages.CorrectQuestionAnswerInputTemplate, correctQuestionsAnswerIndexed.Index + 1, correctQuestionsAnswerIndexed.Item));
        }

        var outputStrings = new List<string>
        {
            CreateTestMessages.TestSavedSuccessfullyOutput
        };


        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }

    private (string testName, List<int> correctQuestionsAnswers) ReadTestInfo()
    {
       var response = testsHttpClientApi.GetAsync().Result;
       Dictionary<string, GetTestApiResponse> testNamesToTestsDict = response == null
           ? new Dictionary<string, GetTestApiResponse>()
           : response!.TestNameToTestDict;

        return (ReadTestName(testNamesToTestsDict), ReadTestCorrectQuestionsAnswers());
    }
    
    private void SaveTest(string testName, List<int> testCorrectQuestionsAnswers)
    {
        CreateTestApiRequest testRequest = new CreateTestApiRequest(testName, testCorrectQuestionsAnswers);

        var response = testsHttpClientApi.CreateAsync(testRequest).Result;
    }
    
    private string ReadTestName(Dictionary<string, GetTestApiResponse> testsCorrectQuestionsAnswers)
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