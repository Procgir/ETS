namespace ElectronicTestSystem.ConsoleApi.Contracts.Marks;

public record CalculateMarksApiResponse(List<CalculateMarksUserMarkResponse> UserMarks);

public record CalculateMarksUserMarkResponse(string UserName, int Mark);