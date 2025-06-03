using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.DomainUnitTests.Testings;

public static class TestingUserAnswersData
{
    public static TestingUserAnswers GetUserAnswersAllCorrect(Test test)
    {
        var userAnswers = new List<UserAnswer>();
        foreach (var question in test.Questions)
        {
            userAnswers.Add(new UserAnswer(question.Id, question.TrueAnswerNumber));
        }

        return TestingUserAnswers
            .Create(Guid.NewGuid(), Guid.NewGuid(), userAnswers.ToArray(), null);
    }
    
    public static TestingUserAnswers GetUserAnswersAllIncorrect(Test test)
    {
        var userAnswers = new List<UserAnswer>();
        foreach (var question in test.Questions)
        {
            userAnswers.Add(new UserAnswer(question.Id, new TestQuestionAnswerNumber(question.TrueAnswerNumber.Value + 1)));
        }

        return TestingUserAnswers
            .Create(Guid.NewGuid(), Guid.NewGuid(), userAnswers.ToArray(), null);
    }
    
    public static TestingUserAnswers GetUserAnswersWithLessCountThenQuestions(Test test, int lessCount = 1)
    {
        var userAnswers = new List<UserAnswer>();
        foreach (var question in test.Questions.Skip(lessCount))
        {
            userAnswers.Add(new UserAnswer(question.Id, new TestQuestionAnswerNumber(question.TrueAnswerNumber.Value + 1)));
        }

        return TestingUserAnswers
            .Create(Guid.NewGuid(), Guid.NewGuid(), userAnswers.ToArray(), null);
    }
}