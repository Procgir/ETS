using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Tests.UpdateTest;

public record UpdateTestCommand(
    Guid TestId,
    string Subject,
    string Theme,
    UpdateTestCommandQuestion[] Questions,
    Guid UserId) : ICommand;

public record UpdateTestCommandQuestion(
    string Body,
    string[] AnswersOptions,
    int TrueAnswerNumber);
