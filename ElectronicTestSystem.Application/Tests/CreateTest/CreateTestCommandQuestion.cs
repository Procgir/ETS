namespace ElectronicTestSystem.Application.Tests.CreateTest;

public record CreateTestCommandQuestion(
    string Body,
    string[] AnswersOptions,
    int TrueAnswerNumber);