using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV1.Commands.CalculateMarkLinear.Errors;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.ConsoleAppV1.Commands.CalculateMarkLinear;

public class CalculateMarkLinearCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
    : ConsoleCommand(app, ConsoleCommandNames.CalculateMarkLinear)
{
    public override void Execute()
    {
        TestDto testDto = null!;
        if (!App.TryDoActionWithRerenderWithOutputStrings(
                (out List<string> stringsForOutput) => TryGetTest(out testDto, out stringsForOutput), true, true))
        {
            Console.WriteLine(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToCreateTest);
            return;
        }

        GroupDto groupDto = null!;
        if (!App.TryDoActionWithRerenderWithOutputStrings(
                (out List<string> stringsForOutput) => TryGetGroupInfo(out groupDto, out stringsForOutput), false, true))
        {
            Console.WriteLine(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToCreateGroup);
            return;
        }

        List<UserAnswersDto> usersAnswers = null!;
        if (!App.TryDoActionWithRerenderWithOutputStrings(
                (out List<string> stringsForOutput) => TryReadUsersAnswers(testDto, groupDto, 
                    out usersAnswers, out stringsForOutput), false, true))
        {
            Console.WriteLine(CalculateMarkLinearCommandErrorMessages.CantCalculateMarkNeedToEnterAllUsersAnswers);
            return;
        }

        var userMarks = markCalculatorFacade.CalculateMarks(testDto.CorrectQuestionsAnswers, usersAnswers);
        
        var userMarksOutputStrings = new List<string>(userMarks.Count + 1);
        userMarksOutputStrings.Add(CalculateMarkLinearCommandMessages.CalculatedUserMark);
        userMarksOutputStrings.AddRange(userMarks.WithIndex().Select((um, i) => $"{um.Item.UserName} {um.Item.Mark}"));
        
        App.ShowWrapped(userMarksOutputStrings, false, true);
    }

    private bool TryGetGroupInfo(out GroupDto groupDto, out List<string> stringsForOutput)
    {
        stringsForOutput = new List<string>();

        var groupNameToGroupsDict = markCalculatorFacade.GetAllGroups();

        bool haveGroups = groupNameToGroupsDict.Count > 0;
        if (!haveGroups)
        {
            groupDto = GroupDto.Empty;
            stringsForOutput = null;
            return false;
        }
        
        (int chosenGroupNumber, string chosenGroupName) = App.ChooseFromListOptions(
            CalculateMarkLinearCommandMessages.ChooseGroup,
            groupNameToGroupsDict.Keys.ToList());

        groupDto = groupNameToGroupsDict[chosenGroupName];
        
        stringsForOutput.Add(CalculateMarkLinearCommandMessages.SelectedGroup);
        stringsForOutput.Add($"{chosenGroupNumber}.{chosenGroupName}");
        
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
        out List<UserAnswersDto> usersAnswers, out List<string> stringsForOutput)
    {
        stringsForOutput = new List<string>();
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
                
        stringsForOutput.Add(CalculateMarkLinearCommandMessages.EnteredAnswers);
        foreach (var userAnswerWithIndex in usersAnswers.WithIndex())
        {
            stringsForOutput.Add($"{userAnswerWithIndex.Index + 1}.{userAnswerWithIndex.Item.UserName}:[{string.Join(',', userAnswerWithIndex.Item.Answers)}]");
        }

        return true;
    }

    private bool TryGetTest(out TestDto testDto, out List<string> stringsForOutput)
    {
        stringsForOutput = new List<string>();
        Dictionary<string, TestDto> testNamesToTests = markCalculatorFacade.GetAllTests();

        Console.WriteLine(CalculateMarkLinearCommandMessages.EnterCorrectQuestionsAnswers);
        
        bool haveTests = testNamesToTests.Count > 0;
        if (!haveTests)
        {
            testDto = TestDto.Empty();
            return false;
        }
        
        (int chosenTestNumber, string chosenTestName) = App.ChooseFromListOptions(CalculateMarkLinearCommandMessages.ChooseTest, testNamesToTests.Keys.ToList());

        testDto = testNamesToTests[chosenTestName];
        
        stringsForOutput.Add(CalculateMarkLinearCommandMessages.SelectedTest);
        stringsForOutput.Add($"{chosenTestNumber}.{chosenTestName}");
        
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