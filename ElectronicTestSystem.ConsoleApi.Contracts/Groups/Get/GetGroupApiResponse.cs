namespace ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;

public record GetGroupApiResponse(string Name, List<GetGroupUserApiResponse> Users);