using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.SDK.Extensions;

namespace ElectronicTestSystem.Domain.Testings.Calculators;

public class MarkCalculator : IMarkCalculator
{
    public Mark CalculateMark(List<int> testsCorrectAnswers, List<int> userAnswersIndexes)
    {
        int correctAnswersCount = CalculateCorrectAnswers(testsCorrectAnswers, userAnswersIndexes);
        int totalAnswers = testsCorrectAnswers.Count;

        int correctAnswersPercentage = GetPercent(correctAnswersCount, totalAnswers);

        return CalculateMarkInner(correctAnswersPercentage);
    }
    
    public List<(string UserName, Mark Mark)> CalculateMarks(List<int> testsCorrectAnswers, List<TestingUserAnswers> usersAnswers)
    {
        List<(string, Mark)> nameMarkPairs = new List<(string, Mark)>(usersAnswers.Count);
        
        foreach (TestingUserAnswers userAnswers in usersAnswers)
        {
            Mark mark = CalculateMark(testsCorrectAnswers, userAnswers.AnswersNumbers);
            nameMarkPairs.Add((userAnswers.UserName,mark));
        }

        return nameMarkPairs;
    }

    public TestingResult[] CalculateMarks(IReadOnlyCollection<TestQuestion> questions, TestingUserAnswers[] usersAnswers)
    {
        if (questions.Count != usersAnswers.Length)
        {
            throw new InvalidOperationException("Questions and answers must have the same length");
        }
        
        var results = new TestingResult[usersAnswers.Length];
        var questionsCorrectAnswers = questions
            .OrderBy(q => q.Id)
            .Select(q => q.TrueAnswerNumber.Value)
            .ToList();
        
        foreach (var userAnswersIndexed in usersAnswers.WithIndex())
        {
            var userAnswersOrderedById = userAnswersIndexed
                .Item
                .Answers
                .OrderBy(a => a.QuestionId)
                .Select(a => a.AnswerNumber.Value)
                .ToList();

            var correctAnswersCount = userAnswersOrderedById.Intersect(questionsCorrectAnswers).Sum();
            var incorrectAnswersCount = questions.Count - correctAnswersCount;
            var totalCount = questions.Count;
            var mark = CalculateMark(questionsCorrectAnswers, userAnswersOrderedById);
            
            results[userAnswersIndexed.Index] = new TestingResult(
                userAnswersIndexed.Item.UserId, 
                correctAnswersCount, 
                incorrectAnswersCount, 
                totalCount, 
                mark);
        }

        return results;
    }

    private Mark CalculateMarkInner(int correctAnswersPercentage)
    {
        switch (correctAnswersPercentage)
        {
            case < 61: return Mark.Two;
            case < 75: return Mark.Three;
            case < 91: return Mark.Four;
            default: return Mark.Five;
        }
    }
    
    private int CalculateCorrectAnswers(List<int> correctAnswers, List<int> userAnswers)
    {
        int correctAnswersCount = 0;
        for (int i = 0; i < correctAnswers.Count; i++)
        {
            if (correctAnswers[i] == userAnswers[i])
            {
                correctAnswersCount = correctAnswersCount + 1;
            }
        }

        return correctAnswersCount;
    }
    
    private int GetPercent(int value, int maxValue)
    {
        return 100 * value / maxValue;
    }
}