namespace ElectronicTestSystem.WebApi.Controllers.Tests;

public sealed record CreateTestRequestQuestion(string Text,
    string[] AnswersOptions,
    int TrueAnswerNumber);