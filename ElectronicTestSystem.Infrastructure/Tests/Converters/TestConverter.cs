using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.SDK.Converters;

namespace ElectronicTestSystem.Infrastructure.Tests.Converters;

public class TestConverter : Converter<Test>
{
    public override string DictKey(Test item) => item.Name;

    public override string Serialize(Test test)
    {
        return $"{test.Name}{SeparatorSymbol}{string.Join(SeparatorSymbol, test.CorrectQuestionsAnswers)}";
    }
    
    public override Test Deserialize(string rawTextTest)
    {
        string[] rawTextTestNameUserAnswers = rawTextTest.Split(SeparatorSymbol);
        
        string testName = rawTextTestNameUserAnswers[0];
        List<int> userAnswers = rawTextTestNameUserAnswers
            .Skip(1)
            .Select(int.Parse)
            .ToList();

        return Test.Create(testName,  userAnswers);
    }
}