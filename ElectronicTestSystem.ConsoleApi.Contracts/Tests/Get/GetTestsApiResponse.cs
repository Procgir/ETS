namespace ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;

public record GetTestsApiResponse(Dictionary<string, GetTestApiResponse> TestNameToTestDict);