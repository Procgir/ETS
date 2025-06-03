using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Testings;

public class TestingUserAnswers : Entity
{
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public string UserName { get; set; }
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<int> AnswersNumbers => Answers.Select(a => a.AnswerNumber.Value).ToList();
    
    private TestingUserAnswers(){}

    public Guid UserId { get; }
    public Guid TestingId { get; }
    public UserAnswer[] Answers { get; private set; }
    public Mark? Mark { get; } 

    internal TestingUserAnswers(
        Guid id,
        Guid testingId,
        Guid userId,
        string userName, 
        UserAnswer[] answers,
        Mark? mark) : base(id)
    {
        UserId = userId;
        UserName = userName;
        Answers = answers;
        Mark = mark;
        TestingId = testingId;
        Answers = answers;
    }
    
    internal TestingUserAnswers(
        Guid id,
        Guid testingId,
        Guid userId,
        UserAnswer[] answers,
        Mark? mark) : base(id)
    {
        UserId = userId;
        UserName = string.Empty;
        Answers = answers;
        Mark = mark;
        TestingId = testingId;
        Answers = answers;
    }

    public void Change(UserAnswer[] answers)
    {
        Answers = answers;
    }
    
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public static TestingUserAnswers Create(string userName, List<int> answers) 
        =>  new (
            Guid.NewGuid(),
            Guid.Empty,
            Guid.Empty, 
            userName,
            answers.Select(a
                => new UserAnswer(Guid.Empty, new TestQuestionAnswerNumber(a))).ToArray(),
            null);
    
    public static TestingUserAnswers Create(
        Guid testingId,
        Guid userId,
        UserAnswer[] answers, 
        Mark? mark) => new (Guid.NewGuid(),testingId, userId, answers, mark);
}