using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.Domain.Tests;

public sealed class TestQuestion : Entity
{
    private TestQuestion()
    {
        
    }
    internal TestQuestion(Guid id,
        Guid testId,
        TestQuestionBody body, 
        List<TestQuestionAnswerOption> answers,
        TestQuestionAnswerNumber trueAnswerNumber) : base(id)
    {
        TestId = testId;
        Body = body;
        Answers = answers;
        TrueAnswerNumber = trueAnswerNumber;
    }

    public Guid TestId { get; }
    public TestQuestionBody Body { get; private set; }
    public List<TestQuestionAnswerOption> Answers { get; }
    public TestQuestionAnswerNumber TrueAnswerNumber { get; private set; }

    internal static TestQuestion Create(Guid testId, string body, string[] answersOptions, int trueAnswerNumber)
    {
        var options = answersOptions
            .WithIndex()
            .Select(i
                => new TestQuestionAnswerOption(i.Item, new TestQuestionAnswerNumber(i.Index))).ToList();
        
        return new TestQuestion(Guid.NewGuid(), testId, new TestQuestionBody(body), options, new TestQuestionAnswerNumber(trueAnswerNumber));
    }
}