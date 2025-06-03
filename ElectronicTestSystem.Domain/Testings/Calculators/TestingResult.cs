namespace ElectronicTestSystem.Domain.Testings.Calculators;

public record TestingResult(
    Guid UserId,
    int CorrectAnswersCount,
    int IncorrectAnswersCount,
    int TotalAnswersCount,
    Mark Mark)
{
    public static TestingResult Empty(Guid userId) => new TestingResult(userId, 0, 0, 0, Mark.One);
}