using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV2.Commands.CalculateMarkLinear.Errors;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Results;

namespace ElectronicTestSystem.ConsoleAppV2.Commands.CalculateMarkLinear;

public class CalculateMarkLinearCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
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
        AddStage(CalculateMarkLinearCommandMessages.CalculateMarkForGroupFourthStageName,() => FourthStageDo(testDto, usersAnswers));
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
        foreach (var userName in usersAnswers.WithIndex())
        {
            inputStrings.Add(string.Format(CalculateMarkLinearCommandMessages.UserAnswersInputTemplate, userName.Index + 1, userName.Item));
        }

        var outputStrings = new List<string>
        {
            CalculateMarkLinearCommandMessages.UsersAnswersEnteredSuccessfully
        };

        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }
    
    private Result<ConsoleCommandsStageResult> FourthStageDo(TestDto testDto, List<UserAnswersDto> usersAnswers)
    {
        var userMarks = markCalculatorFacade.CalculateMarks(testDto.CorrectQuestionsAnswers, usersAnswers);
        
        var result = GetFourthStageResult(userMarks);

        return Result<ConsoleCommandsStageResult>.Success(result);
    }
    
    private ConsoleCommandsStageResult GetFourthStageResult(List<UserMarkDto> usersMarks)
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
        var groupNameToGroupsDict = markCalculatorFacade.GetAllGroups();

        bool haveGroups = groupNameToGroupsDict.Count > 0;
        if (!haveGroups)
        {
            groupDto = GroupDto.Empty;
            return false;
        }
        
        (int chosenGroupNumber, string chosenGroupName) = App.ChooseFromListOptions(
            CalculateMarkLinearCommandMessages.ChooseGroup,
            groupNameToGroupsDict.Keys.ToList());

        groupDto = groupNameToGroupsDict[chosenGroupName];
        
        return true;
    }

    private static void ShowMarks(List<UserMarkDto> nameMarkPairs)
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
        Dictionary<string, TestDto> testNamesToTests = markCalculatorFacade.GetAllTests();

        bool haveTests = testNamesToTests.Count > 0;
        if (!haveTests)
        {
            testDto = TestDto.Empty();
            return false;
        }
        
        (int chosenTestNumber, string chosenTestName) = App.ChooseFromListOptions(CalculateMarkLinearCommandMessages.ChooseTest, testNamesToTests.Keys.ToList());

        testDto = testNamesToTests[chosenTestName];
        
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