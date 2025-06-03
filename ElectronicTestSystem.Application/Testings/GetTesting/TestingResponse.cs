namespace ElectronicTestSystem.Application.Testings.GetTesting;

public record TestingResponse(
    string GroupName,
    string TestName,
    DateTime CreatedAt,
    DateTime EndedAt,
    int DoneCount,
    int AllCount,
    TestingUserResultResponse[] UserResults);
    
public record TestingUserResultResponse(
    string UserName,
    int AnswersCount,
    int IncorrectAnswersCount,
    int CorrectAnswersCount,
    int Mark);