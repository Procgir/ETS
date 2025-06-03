using ElectronicTestSystem.Domain.Testings;

namespace ElectronicTestSystem.Application.Dto;

public class UserAnswersDto(string userName, List<int> answers)
{
    public string UserName { get; } = userName;
    public List<int> Answers { get; } = answers;

    internal static UserAnswersDto From(TestingUserAnswers testingUserAnswers) => new UserAnswersDto(testingUserAnswers.UserName, testingUserAnswers.AnswersNumbers);
}