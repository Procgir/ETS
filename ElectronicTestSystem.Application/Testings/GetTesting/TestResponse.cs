namespace ElectronicTestSystem.Application.Testings.GetTesting;

public sealed record TestResponse(
    Guid Id,
    string Subject,
    string Theme,
    DateTime CreatedAt,
    TestQuestionResponse[] Questions);
    
public sealed record TestQuestionResponse(
    Guid Id,
    string Body,
    string[] AnswersOptions,
    int TrueAnswerNumber);