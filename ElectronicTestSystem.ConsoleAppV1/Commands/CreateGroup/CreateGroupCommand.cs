using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Application.MarkCalculators;
using ElectronicTestSystem.ConsoleFrameworkV1.Abstractions.Commands;

namespace ElectronicTestSystem.ConsoleAppV1.Commands.CreateGroup;

public class CreateGroupCommand(
    ConsoleCommandApp app,
    IMarkCalculatorFacade markCalculatorFacade)
: ConsoleCommand(app, ConsoleCommandNames.CreateGroup)
{
    public override void Execute()
    {
        var groupInfo = ReadGroupInfo();
        
        SaveGroup(groupInfo.groupName, groupInfo.enteredUsers);
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