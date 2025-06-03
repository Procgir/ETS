namespace ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;

public record GetGroupsApiResponse(Dictionary<string, GetGroupApiResponse> GroupNameToGroupDict);