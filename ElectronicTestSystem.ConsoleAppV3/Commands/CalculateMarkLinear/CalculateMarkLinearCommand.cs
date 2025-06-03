using ElectronicTestSystem.ConsoleApi.Clients.Groups;
using ElectronicTestSystem.ConsoleApi.Clients.Marks;
using ElectronicTestSystem.ConsoleApi.Clients.Tests;
using ElectronicTestSystem.ConsoleApi.Contracts.Marks;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;
using ElectronicTestSystem.ConsoleAppV3.Commands.CalculateMarkLinear.Dtos;
using ElectronicTestSystem.ConsoleAppV3.Commands.CalculateMarkLinear.Errors;
using ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup.Dtos;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Results;

namespace ElectronicTestSystem.ConsoleAppV3.Commands.CalculateMarkLinear;

public class CalculateMarkLinearCommand(
    ConsoleCommandApp app,
    IGroupsHttpClientApi groupsHttpClientApi,
    ITestsHttpClientApi testsHttpClientApi,
    IMarksHttpClientApi marksHttpClientApi)
    : ConsoleCommand(app, ConsoleCommandNames.CalculateMarkLinear)
{
    protected override void InitializeStages()
    {
        TestDto testDto = null!;
        AddStage(CalculateMarkLinearCommandMessages.SelectTestFirstStageName, () => FirstStageDo(out testDto));
        GroupDto groupDto = null!;
        AddStage(CalculateMarkLinearCommandMessages.SelectGroupSecondStageName,() => SecondStageDo(out groupDto));
        List<UserAnswersDto> usersAnswers = null!;
        AddStage(CalculateMarkLinearCommandMessages.EnterUsersAnswersThirdStageName,() => ThirdStageDo(testDto, groupDto, out usersAnswers));
        AddStage(CalculateMarkLinearCommandMessages.CalculateMarkForGroupFourthStageName,() => FourthStageDo(testDto, groupDto, usersAnswers));
    }

    private Result<ConsoleCommandsStageResult> FirstStageDo(out TestDto testDto)
    {
        if (!TryGetTest(out testDto))
        {
            return Result<ConsoleCommandsStageResult>.Fail(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToCreateTest);
        }

        var result = GetFirstStageResult(testDto);
        return Result<ConsoleCommandsStageResult>.Success(result);
    }
    
    private ConsoleCommandsStageResult GetFirstStageResult(TestDto testDto)
    {
        var inputStrings = new List<string>();
 
        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.SelectedTestNameInputTemplate, testDto.Name));

        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.TestCorrectQuestionsAnswersTitleInput));
        foreach (var correctQuestionAnswer in testDto.CorrectQuestionsAnswers.WithIndex())
        {
            inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.TestCorrectQuestionsAnswerInputTemplate, correctQuestionAnswer.Index + 1, correctQuestionAnswer.Item));
        }

        var outputStrings = new List<string>
        {
            CalculateMarkLinearCommandMessages.TestChosenSuccessfully
        };

        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }

    private Result<ConsoleCommandsStageResult> SecondStageDo(out GroupDto groupDto)
    {
        if (!TryGetGroupInfo(out groupDto))
        {
            return Result<ConsoleCommandsStageResult>.Fail(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToCreateGroup);
        }

        var result = GetSecondStageResult(groupDto);
        return Result<ConsoleCommandsStageResult>.Success(result);
    }
    
    private ConsoleCommandsStageResult GetSecondStageResult(GroupDto groupDto)
    {
        var inputStrings = new List<string>();
 
        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.SelectedGroupNameInputTemplate, groupDto.Name));

        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.UsersGroupTitleInput));
        foreach (var userName in groupDto.UsersNames.WithIndex())
        {
            inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.UsersGroupInputTemplate, userName.Index + 1, userName.Item));
        }

        var outputStrings = new List<string>
        {
            CalculateMarkLinearCommandMessages.GroupChosenSuccessfully
        };

        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }
    
    private Result<ConsoleCommandsStageResult> ThirdStageDo(TestDto testDto, GroupDto groupDto, out List<UserAnswersDto> usersAnswers)
    {
        if (!TryReadUsersAnswers(testDto, groupDto, 
                out usersAnswers))
        {
            return Result<ConsoleCommandsStageResult>.Fail(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToEnterAllUsersAnswers);
        }

        var result = GetThirdStageResult(usersAnswers);

        return Result<ConsoleCommandsStageResult>.Success(result);
    }
    
    private ConsoleCommandsStageResult GetThirdStageResult(List<UserAnswersDto> usersAnswers)
    {
        var inputStrings = new List<string>();
 
        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.EnteredUsersAnswersTitleInput));
        foreach (var userAnswers in usersAnswers.WithIndex())
        {
            inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.UserAnswersInputTemplate, userAnswers.Index + 1, userAnswers.Item.UserName, string.Join(',', userAnswers.Item.Answers)));
        }

        var outputStrings = new List<string>
        {
            CalculateMarkLinearCommandMessages.UsersAnswersEnteredSuccessfully
        };

        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }
    
    private Result<ConsoleCommandsStageResult> FourthStageDo(TestDto testDto, GroupDto groupDto,
        List<UserAnswersDto> usersAnswers)
    {
        var request = new CalculateMarksApiRequest(usersAnswers
            .Select(ua => new CalculateMarksUserAnswersRequest(ua.UserName, ua.Answers))
            .ToList());
        
        var response = marksHttpClientApi.CalculateAsync(testDto.Name, groupDto.Name, request).Result;
        if (response == null)
        {
            return Result<ConsoleCommandsStageResult>.Fail(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkUnknownError);
        }

        var result = GetFourthStageResult(response.UserMarks);

        return Result<ConsoleCommandsStageResult>.Success(result);
    }
    
    private ConsoleCommandsStageResult GetFourthStageResult(List<CalculateMarksUserMarkResponse> usersMarks)
    {
        var inputStrings = new List<string>(usersMarks.Count + 1);
        
        inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.EnteredUsersAnswersTitleInput));
        foreach (var userMark in usersMarks.WithIndex())
        {
            inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.UserMarkInputTemplate, userMark.Index + 1, userMark.Item.UserName, userMark.Item.Mark));
        }

        var outputStrings = new List<string>
        {
            CalculateMarkLinearCommandMessages.UserMarkCalculatedSuccessfully
        };

        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }

    private bool TryGetGroupInfo(out GroupDto groupDto)
    {
        var response = groupsHttpClientApi.GetAsync().Result;
        if (response == null)
        {
            groupDto = GroupDto.Empty();
            return false;
        }
        
        var groupNameToGroupsDict = response.GroupNameToGroupDict;
        bool haveGroups = groupNameToGroupsDict.Count > 0;
        if (!haveGroups)
        {
            groupDto = GroupDto.Empty();
            return false;
        }
        
        (int chosenGroupNumber, string chosenGroupName) = App.ChooseFromListOptions(
            CalculateMarkLinearCommandMessages.ChooseGroup,
            groupNameToGroupsDict.Keys.ToList());

        var groupResponse = groupNameToGroupsDict[chosenGroupName];
        
        groupDto = new GroupDto(groupResponse.Name, groupResponse.Users.Select(u => u.Name).ToList());
        
        return true;
    }

    private static void ShowMarks(List<CalculateMarksUserMarkResponse> nameMarkPairs)
    {
        foreach (var indexedPair in nameMarkPairs.WithIndex())
        {
            Console.WriteLine($"{indexedPair.Index + 1} {indexedPair.Item.UserName} {indexedPair.Item.Mark}");
        }
    }

    private bool TryReadUserAnswers(List<int> testCorrectQuestionsAnswers, out List<int> userAnswers)
    {
        return App.TryReadInputListUntilStopOrEnd(
            CalculateMarkLinearCommandMessages.EnterUserAnswerNumber, 
            (input, count) => count == testCorrectQuestionsAnswers.Count,
            CalculateMarkLinearCommandMessages.AllAnswersEntered,
            (string? input, out int parsedInput) => int.TryParse(input, out parsedInput),
            CalculateMarkLinearCommandMessages.IncorrectInputFormatNeedToRetryTemplate,
            CalculateMarkLinearCommandMessages.CantCalculateMarkQuestionsCountNotEqualUserAnswersCount,
            out userAnswers);
    }

    private bool TryReadUsersAnswers(TestDto testDto, GroupDto groupDto,
        out List<UserAnswersDto> usersAnswers)
    {
        usersAnswers = new List<UserAnswersDto>();

        foreach (var userName in groupDto.UsersNames)
        {
            if (!TryReadUserAnswers(userName, testDto.CorrectQuestionsAnswers, out var enteredAnswers))
            {
                return false;
            }

            UserAnswersDto userAnswers = new UserAnswersDto(userName, enteredAnswers);
            usersAnswers.Add(userAnswers);
        }

        return true;
    }

    private bool TryGetTest(out TestDto testDto)
    {
        var response = testsHttpClientApi.GetAsync().Result;
        if (response == null)
        {
            testDto = TestDto.Empty();
            return false;
        }
        
        Dictionary<string, GetTestApiResponse> testNamesToTests = response.TestNameToTestDict;
        bool haveTests = testNamesToTests.Count > 0;
        if (!haveTests)
        {
            testDto = TestDto.Empty();
            return false;
        }
        
        (int chosenTestNumber, string chosenTestName) = App.ChooseFromListOptions(CalculateMarkLinearCommandMessages.ChooseTest, testNamesToTests.Keys.ToList());

        var testResponse = testNamesToTests[chosenTestName];
        
        testDto = new TestDto(testResponse.Name, testResponse.CorrectQuestionsAnswers);
        
        return true;
    }
    
    private bool TryReadUserAnswers(string userName, List<int> testCorrectQuestionsAnswers, out List<int> enteredAnswers)
    {
        Console.WriteLine(userName);
        Console.WriteLine(CalculateMarkLinearCommandMessages.EnterUserAnswers);
        
        if (!TryReadUserAnswers(testCorrectQuestionsAnswers, out List<int> parsedAnswers ))
        {
            enteredAnswers = new List<int>(0);
            Console.WriteLine(CalculateMarkLinearCommandMessages.CantCalculateMarkQuestionsCountNotEqualUserAnswersCount);
            return false;
        }
        
        enteredAnswers = parsedAnswers;
    
        return true;
    }
}