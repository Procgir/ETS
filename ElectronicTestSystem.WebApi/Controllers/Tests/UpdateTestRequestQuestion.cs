namespace ElectronicTestSystem.WebApi.Controllers.Tests;

public record UpdateTestRequestQuestion(
    Guid Id,
    string Text,
    string[] AnswersOptions,
    int TrueAnswerNumber);