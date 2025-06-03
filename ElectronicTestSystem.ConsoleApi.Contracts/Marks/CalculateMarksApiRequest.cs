namespace ElectronicTestSystem.ConsoleApi.Contracts.Marks;

public record CalculateMarksApiRequest(List<CalculateMarksUserAnswersRequest> UsersAnswers);

public record CalculateMarksUserAnswersRequest(string UserName, List<int> Answers);