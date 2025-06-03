using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.ConsoleAppV1.Commands.CalculateMark;

public class CalculateMarkCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
    : ConsoleCommand(app, ConsoleCommandNames.CalculateMark)
{
    public override void Execute()
    {
        if (!TryGetTestCorrectQuestionsAnswers(out var chosenCorrectQuestionsAnswersIndexes)) return;

        Dictionary<string, GroupDto> groupsNamesUsersNames = markCalculatorFacade.GetAllGroups();

        bool haveSavedGroups = groupsNamesUsersNames.Count > 0;
        bool doManualInputUserNames = 
            haveSavedGroups &&
            App.ReadUserYesOrNoAnswer(CalculateMarkCommandMessages.UseSavedGroupsQuestion).IsNo();
        
        if (doManualInputUserNames)
        {
            TryCalculateMarkForNewGroup(groupsNamesUsersNames,  chosenCorrectQuestionsAnswersIndexes);
        }
        else
        {
            TryCalculateMarkForExistingGroup(groupsNamesUsersNames, chosenCorrectQuestionsAnswersIndexes);
        }
    }
    
    private bool TryCalculateMarkForNewGroup(Dictionary<string, GroupDto> groups, List<int> chosenCorrectQuestionsAnswers)
    {
        if (!TryReadManualUsersAndAnswers(chosenCorrectQuestionsAnswers,
                out var usersNames,
                out var usersAnswers))
        {
            return false;
        }
            
        var userMarks = markCalculatorFacade.CalculateMarks(chosenCorrectQuestionsAnswers, usersAnswers);

        ShowMarks(userMarks);

        if (App.ReadUserYesOrNoAnswer(CalculateMarkCommandMessages.SavedGroupsToFileQuestion).IsYes())
        {
            SaveNewGroup(groups, usersNames);
        }
        
        return true;
    }

    private void SaveNewGroup(Dictionary<string, GroupDto> groups, List<UserDto> manualEnteredUsers)
    {
        string groupName = ReadGroupName(groups);

        GroupDto group = new GroupDto(groupName, 
            manualEnteredUsers.Select(u => u.Name).ToList());

        markCalculatorFacade.SaveGroup(group);
    }

    private static void ShowMarks(List<UserMarkDto> nameMarkPairs)
    {
        foreach (var indexedPair in nameMarkPairs.WithIndex())
        {
            Console.WriteLine($"{indexedPair.Index + 1} {indexedPair.Item.UserName} {indexedPair.Item.Mark}");
        }
    }

    private bool TryReadManualUsersAndAnswers(List<int> chosenCorrectQuestionsAnswers, 
        out List<UserDto> manualEnteredUsers,
        out List<UserAnswersDto> usersAnswers)
    {
        manualEnteredUsers = new List<UserDto>();
        usersAnswers = new List<UserAnswersDto>();
        
        while (true)
        {
            Console.WriteLine();
            
            string currentProccessedUserName = App.ReadInputUntilValid(
                CalculateMarkCommandMessages.EnterUserName, 
                CheckDelegateDefaults.SucceededCheck);
            
            UserDto user = new UserDto(currentProccessedUserName);
            manualEnteredUsers.Add(user);
            
            if (!TryReadUserAnswers(chosenCorrectQuestionsAnswers, out var currentUserAnswers))
            {
                return false;
            }

            UserAnswersDto userAnswer = new UserAnswersDto(currentProccessedUserName, currentUserAnswers);
            usersAnswers.Add(userAnswer);
            
            Console.WriteLine();

            if (App.ReadUserYesOrNoAnswer(CalculateMarkCommandMessages.ContinueQuestion).IsNo())
            {
                return true;
            }
        }
    }

    private bool TryReadUserAnswers(List<int> testCorrectQuestionsAnswers, out List<int> userAnswers)
    {
        return App.TryReadInputListUntilStopOrEnd(
            CalculateMarkCommandMessages.EnterUserAnswerNumber, 
            (input, count) => count == testCorrectQuestionsAnswers.Count,
            CalculateMarkCommandMessages.AllAnswersEntered,
            (string? input, out int parsedInput) => int.TryParse(input, out parsedInput),
            CalculateMarkCommandMessages.IncorrectInputFormatNeedToRetryTemplate,
            CalculateMarkCommandMessages.CantCalculateMarkQuestionsCountNotEqualUserAnswersCount,
            out userAnswers);
    }

    private bool TryCalculateMarkForExistingGroup(Dictionary<string, GroupDto> groupsUsersNames, List<int> correctQuestionsAnswersIndexes)
    {
        (int chosenGroupNumber, string chosenGroupName) = App.ChooseFromListOptions(CalculateMarkCommandMessages.ChooseGroup,
            groupsUsersNames.Keys.ToList());

        if (!TryReadUsersAnswers(groupsUsersNames, correctQuestionsAnswersIndexes, chosenGroupName,
                out var usersAnswers))
        {
            return false;
        }

        var userMarks = markCalculatorFacade.CalculateMarks(correctQuestionsAnswersIndexes, usersAnswers);
        
        ShowMarks(userMarks);
        
        return true;
    }

    private bool TryReadUsersAnswers(Dictionary<string, GroupDto> groupsUsersNames, List<int> correctQuestionsAnswersIndexes, string chosenGroupName,
        out List<UserAnswersDto> usersAnswers)
    {
        usersAnswers = new List<UserAnswersDto>();

        foreach (var userName in groupsUsersNames[chosenGroupName].UsersNames)
        {
            if (!TryReadUserAnswers(userName, correctQuestionsAnswersIndexes, out var enteredAnswers))
            {
                return false;
            }

            UserAnswersDto userAnswers = new UserAnswersDto(userName, enteredAnswers);
            usersAnswers.Add(userAnswers);
        }

        return true;
    }

    private bool TryGetTestCorrectQuestionsAnswers(out List<int> chosenCorrectQuestionsAnswers)
    {
        Dictionary<string, TestDto> testNamesToTests = markCalculatorFacade.GetAllTests();

        Console.WriteLine(CalculateMarkCommandMessages.EnterCorrectQuestionsAnswers);
        
        bool haveSavedCorrectQuestionsAnswers = testNamesToTests.Count > 0;
        bool doManualInputCorrectQuestionsAnswers = haveSavedCorrectQuestionsAnswers &&
               App.ReadUserYesOrNoAnswer(CalculateMarkCommandMessages.HaveSavedCorrectQuestionsAnswersQuestion)
                   .IsNo();

        if (doManualInputCorrectQuestionsAnswers)
        {
            chosenCorrectQuestionsAnswers = ReadTestCorrectQuestionsAnswers();

            if (App.ReadUserYesOrNoAnswer(CalculateMarkCommandMessages.SaveCorrectQuestionsAnswersToFile).IsYes())
            {
                SaveTest(testNamesToTests, chosenCorrectQuestionsAnswers);
            }
        }
        else
        {
            (int chosenTestNumber, string chosenTestName) = App.ChooseFromListOptions(CalculateMarkCommandMessages.ChooseTest, testNamesToTests.Keys.ToList());

            chosenCorrectQuestionsAnswers = testNamesToTests[chosenTestName].CorrectQuestionsAnswers;
        }

        return chosenCorrectQuestionsAnswers.Count != 0;
    }

    private void SaveTest(Dictionary<string, TestDto> testsCorrectAnswers, List<int> manualEnteredCorrectQuestionsAnswers)
    {
        string testName = ReadTestName(testsCorrectAnswers);
                
        TestDto test = new TestDto(testName, manualEnteredCorrectQuestionsAnswers);

        markCalculatorFacade.SaveTest(test);
    }

    private bool TryReadUserAnswers(string userName, List<int> testCorrectQuestionsAnswers, out List<int> enteredAnswers)
    {
        Console.WriteLine(userName);
        Console.WriteLine(CalculateMarkCommandMessages.EnterUserAnswers);
        
        if (!TryReadUserAnswers(testCorrectQuestionsAnswers, out List<int> parsedAnswers ))
        {
            enteredAnswers = new List<int>(0);
            Console.WriteLine(CalculateMarkCommandMessages.CantCalculateMarkQuestionsCountNotEqualUserAnswersCount);
            return false;
        }
        
        enteredAnswers = parsedAnswers;
    
        return true;
    }

    private string ReadGroupName(Dictionary <string, GroupDto> groupsNamesUsersNames)
    {
        return App.ReadInputUntilValid(CalculateMarkCommandMessages.EnterGroupNameForSaving, (string input, out string errorMessage) =>
        {
            errorMessage = string.Empty;
            if (groupsNamesUsersNames.Count > 0 && groupsNamesUsersNames.ContainsKey(input))
            {
                errorMessage = CalculateMarkCommandMessages.GroupExistsWithSameName;
                return false;
            }

            return true;
        });
    }

    private string ReadTestName(Dictionary<string, TestDto> testsCorrectQuestionsAnswers)
    {
        return App.ReadInputUntilValid(CalculateMarkCommandMessages.EnterTestNameForSaving, (string input, out string errorMessage) =>
        {
            errorMessage = string.Empty;
            if (testsCorrectQuestionsAnswers.Count > 0 && testsCorrectQuestionsAnswers.ContainsKey(input))
            {
                errorMessage = CalculateMarkCommandMessages.TestExistsWithSameName;
                return false;
            }
    
            return true;
        });
    }
    
    private List<int> ReadTestCorrectQuestionsAnswers()
    {
        App.TryReadInputListUntilStopOrEnd(
            CalculateMarkCommandMessages.EnterCorrectAnswerNumber,
            (input, count) => false,
            string.Empty,
            (string? input, out int parsedInput) => int.TryParse(input, out parsedInput),
            CalculateMarkCommandMessages.IncorrectInputFormatNeedToRetryTemplate,
            string.Empty,
            out List<int> testQuestionsAnswers);
    
        return testQuestionsAnswers;
    }
}