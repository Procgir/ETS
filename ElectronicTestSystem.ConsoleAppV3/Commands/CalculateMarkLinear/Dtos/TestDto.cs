namespace ElectronicTestSystem.ConsoleAppV3.Commands.CalculateMarkLinear.Dtos;

public record TestDto(string Name, List<int> CorrectQuestionsAnswers)
{
    public static TestDto Empty() => new TestDto(string.Empty, new List<int>(0));
}