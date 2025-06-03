using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests.Events;
using ElectronicTestSystem.SDK.Extensions;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Tests;

public sealed class Test : Entity
{
    private List<TestQuestion> _questions = new();
    public TestSubject Subject { get; private set; }
    public TestTheme Theme { get; private set; }
    public IReadOnlyCollection<TestQuestion> Questions => _questions.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    
    public Guid AuthorId { get; private set; }
    public List<int> CorrectQuestionsAnswers => Questions.Select(q => q.TrueAnswerNumber.Value).ToList();
    public List<int> CorrectQuestionsAnswersOrderedById => Questions.OrderBy(q=>q.Id).Select(q => q.TrueAnswerNumber.Value).ToList();
    public string Name => Subject.Name;

    private Test()
    {
        
    }
    
    public Result AddQuestion(string body, string[] answersOptions, int trueAnswerNumber)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return Result.Failure(TestErrors.CantAddQuestionWithEmptyBody);
        }

        if (answersOptions.Length == 0)
        {
            return Result.Failure(TestErrors.CantAddQuestionWithEmptyAnswersOptions);
        }

        if (trueAnswerNumber < 1 || answersOptions.Length < trueAnswerNumber)
        {
            return Result.Failure(TestErrors.CantAddQuestionWithIncorrectTrueAnswerNumber);
        }
        
        var question = TestQuestion.Create(Id, body, answersOptions, trueAnswerNumber);
        
        _questions.Add(question);

        return Result.Success();
    }
    
    public void ClearQuestions() => _questions.Clear();
    
    public void Update(
        TestSubject? subject,
        TestTheme? theme,
        TestQuestion[]? questions)
    {
        if (subject != null)
        {
            Subject = subject;
        }
        
        if (theme != null)
        {
            Theme = theme;
        }
        
        if (questions != null)
        {
            _questions = questions.ToList();
        }
    }
    private Test(Guid id, 
        TestSubject subject,
        TestTheme theme,
        List<TestQuestion> questions,
        DateTime createdAt,
        Guid authorId) : base(id)
    {
        Id = id;
        Subject = subject;
        Theme = theme;
        _questions = questions;
        CreatedAt = createdAt;
        AuthorId = authorId;
    }
    
    private Test(Guid id, 
        TestSubject subject,
        TestTheme theme,
        DateTime createdAt,
        Guid authorId) : base(id)
    {
        Id = id;
        Subject = subject;
        Theme = theme;
        CreatedAt = createdAt;
        AuthorId = authorId;
    }

    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public static Test Create(string name, List<int> trueQuestionsAnswersOptions)
    {
        var id = Guid.NewGuid();
        var questions = trueQuestionsAnswersOptions
            .WithIndex()
            .Select(pair 
                => TestQuestion.Create(id, string.Empty, [], pair.Item + 1))
            .ToList();

        var test = new Test(id, new TestSubject(name), new TestTheme(string.Empty), questions, DateTime.Now, Guid.Empty);
        
        test.RaiseDomainEvent(new TestCreatedDomainEvent(test.Id));

        return test;
    }
    
    public static Test Create(
        string subjectName,
        string themeName)
    {
        var id = Guid.NewGuid();
        
        var test = new Test(id, new TestSubject(subjectName), new TestTheme(themeName), DateTime.Now, Guid.Empty);
        
        test.RaiseDomainEvent(new TestCreatedDomainEvent(test.Id));

        return test;
    }
    
    public static Test Create(
        string subjectName,
        string themeName,
        Guid authorId)
    {
        var id = Guid.NewGuid();
        
        var test = new Test(id, new TestSubject(subjectName), new TestTheme(themeName), DateTime.Now, authorId);
        
        test.RaiseDomainEvent(new TestCreatedDomainEvent(test.Id));

        return test;
    }
    
    public static Test Create(
        string subjectName,
        string themeName,
        Guid authorId,
        DateTime createdAt)
    {
        var id = Guid.NewGuid();
        
        var test = new Test(id, new TestSubject(subjectName), new TestTheme(themeName), createdAt, authorId);
        
        test.RaiseDomainEvent(new TestCreatedDomainEvent(test.Id));

        return test;
    }
}