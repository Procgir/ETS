namespace ElectronicTestSystem.Application.Dto;

public sealed class TestResponse
{
    public TestResponse(){}
    public TestResponse(
        Guid id, 
        string subject, 
        string theme, 
        DateTime createdAt, 
        TestQuestionResponse[] questions, 
        Guid authorId)
    {
        Id = id;
        Subject = subject;
        Theme = theme;
        CreatedAt = createdAt;
        Questions = questions;
        AuthorId = authorId;
    }

    public Guid Id { get; internal set; }
    public string Subject { get; internal set; }
    public string Theme { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public Guid AuthorId { get; internal set; }
    public TestQuestionResponse[] Questions { get; internal set; }
}

public sealed class TestQuestionResponse
{
    public TestQuestionResponse() {}
    public TestQuestionResponse(Guid id, string body, string[] answersOptions, int trueAnswerNumber)
    {
        Id = id;
        Body = body;
        AnswersOptions = answersOptions;
        TrueAnswerNumber = trueAnswerNumber;
    }

    public Guid Id { get; internal set; }
    public string Body { get; internal set; }
    public string[] AnswersOptions { get; internal set; }
    public int TrueAnswerNumber { get; internal set; }
}
    