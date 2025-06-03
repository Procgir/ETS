namespace ElectronicTestSystem.ConsoleApi.Contracts.Groups.Create;

public record CreateGroupApiRequest(string Name,List<CreateGroupUserApiRequest> Users);


