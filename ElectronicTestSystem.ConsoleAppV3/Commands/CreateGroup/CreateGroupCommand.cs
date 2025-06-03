using ElectronicTestsSystem.ConsoleFramework.Shared.Extensions;
using ElectronicTestsSystem.ConsoleFramework.Shared.Functions;
using ElectronicTestSystem.ConsoleApi.Clients.Groups;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;
using ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup.Dtos;
using ElectronicTestSystem.ConsoleFrameworkV2.Abstractions.Commands;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Results;

namespace ElectronicTestSystem.ConsoleAppV3.Commands.CreateGroup;

public class CreateGroupCommand(
    ConsoleCommandApp app,
    IGroupsHttpClientApi groupsHttpClientApi)
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
        var response = groupsHttpClientApi.GetAsync().Result;
        
        Dictionary<string, GetGroupApiResponse> groupNameToGroupsDict = response == null
            ? new Dictionary<string, GetGroupApiResponse>()
            : response!.GroupNameToGroupDict;

        return (ReadGroupName(groupNameToGroupsDict), ReadGroupUsers());
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
    
    private string ReadGroupName(Dictionary <string, GetGroupApiResponse> groupNameToGroupDict)
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
        var createGroupUserRequests = usersDto
            .Select(u => new CreateGroupUserApiRequest(u.Name))
            .ToList();
        
        CreateGroupApiRequest request = new CreateGroupApiRequest(groupName, 
            createGroupUserRequests);

        groupsHttpClientApi.CreateAsync(request);
    }
}