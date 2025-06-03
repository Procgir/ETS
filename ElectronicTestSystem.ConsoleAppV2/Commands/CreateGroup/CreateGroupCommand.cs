using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleAppV2.Commands.CreateTest;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Results;

namespace ElectronicTestSystem.ConsoleAppV2.Commands.CreateGroup;

public class CreateGroupCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
: ConsoleCommand(app, ConsoleCommandNames.CreateGroup)
{
    protected override void InitializeStages()
    {
        AddStage(CreateGroupMessages.SingleStageName, SingleStageDo);
    }

    private Result<ConsoleCommandsStageResult> SingleStageDo()
    {
        var groupInfo = ReadGroupInfo();
        
        SaveGroup(groupInfo.groupName, groupInfo.enteredUsers);

        var stageResult = GetStageResult(groupInfo);
        
        return Result<ConsoleCommandsStageResult>.Success(stageResult);
    }
    
    private ConsoleCommandsStageResult GetStageResult((string groupName, List<UserDto> enteredUsers) groupInfo)
    {
        var inputStrings = new List<string>(groupInfo.enteredUsers.Count + 2);
 
        inputStrings.Add(string.Format(CreateGroupMessages.GroupNameInputTemplate, groupInfo.groupName));
        inputStrings.Add(string.Format(CreateGroupMessages.UsersGroupTitleInput));

        foreach (var user in groupInfo.enteredUsers.WithIndex())
        {
            inputStrings.Add(string.Format(CreateGroupMessages.UsersGroupInputTemplate, user.Index + 1, user.Item));
        }

        var outputStrings = new List<string>
        {
            CreateGroupMessages.GroupSavedSuccessfullyOutput
        };


        return new ConsoleCommandsStageResult(inputStrings, outputStrings);
    }
    
    private (string groupName, List<UserDto> enteredUsers) ReadGroupInfo()
    {
        var groups = markCalculatorFacade.GetAllGroups();

        return (ReadGroupName(groups), ReadGroupUsers());
    }

    private List<UserDto> ReadGroupUsers()
    {
        var manualEnteredUsers = new List<UserDto>();

        do
        {
            Console.WriteLine();
            
            string currentUserName = App.ReadInputUntilValid(
                CreateGroupMessages.EnterUserName, 
                CheckDelegateDefaults.SucceededCheck);
            
            UserDto user = new UserDto(currentUserName);
            manualEnteredUsers.Add(user);
        } 
        while (App.ReadUserYesOrNoAnswer(CreateGroupMessages.ContinueQuestion).IsYes());

        return manualEnteredUsers;
    }
    
    private string ReadGroupName(Dictionary <string, GroupDto> groupNameToGroupDict)
    {
        return App.ReadInputUntilValid(CreateGroupMessages.EnterGroupNameForSaving, (string input, out string errorMessage) =>
        {
            errorMessage = string.Empty;
            if (groupNameToGroupDict.Count > 0 && groupNameToGroupDict.ContainsKey(input))
            {
                errorMessage = CreateGroupMessages.GroupExistsWithSameName;
                return false;
            }

            return true;
        });
    }
    
    private void SaveGroup(string groupName, List<UserDto> usersDto)
    {
        GroupDto groupDto = new GroupDto(groupName, 
            usersDto.Select(u => u.Name).ToList());

        markCalculatorFacade.SaveGroup(groupDto);
    }
}