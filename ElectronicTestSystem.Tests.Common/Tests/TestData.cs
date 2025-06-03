using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Tests.Common.Tests;

public static class TestData
{
    public static readonly string SubjectName = "Test subject";
    public static readonly string ThemeName = "Test theme";
    public static readonly Guid AuthorId = Guid.Parse("1a1a1a1a-1a1a-1a1a-1a1a-1a1a1a1a1a1a");
    public static readonly DateTime CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0);
    
    public static readonly Dictionary<string, (string[] AnswersOptions, int TrueAnswerNumber)> Questions = new()
    {
        {"q1", (new[] {"a11", "a12", "a13", "a14"  }, 1)},
        {"q2", (new[] {"a21", "a22", "a23", "a24"  }, 1)},
        {"q3", (new[] {"a31", "a32", "a33", "a34"  }, 1)},
        {"q4", (new[] {"a41", "a42", "a43", "a44"  }, 1)},
        {"q5", (new[] {"a51", "a52", "a53", "a54"  }, 1)},
        {"q6", (new[] {"a61", "a62", "a63", "a64"  }, 1)},
    };

    public static Test GetDefaultTest(Guid? authorId = null)
    {
        var test = Test
            .Create(TestData.SubjectName,
                TestData.ThemeName,
                authorId?? TestData.AuthorId,
                TestData.CreatedAt);
        foreach (var questionBodyDataPair in TestData.Questions)
        {
            var questionBody = questionBodyDataPair.Key;
            var questionInfo = questionBodyDataPair.Value;
            test.AddQuestion(questionBody, questionInfo.AnswersOptions, questionInfo.TrueAnswerNumber);
        }

        return test;
    }

    public static int[] QuestionsCorrectAnswers(Test test)
    {
        return test.Questions.Select(q => q.TrueAnswerNumber.Value).ToArray();
    }
}
