using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.DomainUnitTests.Tests;
using FluentAssertions;

namespace ElectronicTestSystem.DomainUnitTests.Testings.Calculators;

public class MarkCalculatorTests
{
    [Fact]
    public void CalculateMarks_ShouldHaveCorrectTestingResultWith5Mark_WhenAllUserAnswersAreCorrect()
    {
        //Arrange
        var test = TestData.GetDefaultTest();
        var calculator = new MarkCalculator();
        var userAnswers = TestingUserAnswersData.GetUserAnswersAllCorrect(test);

        // Act
        var result = calculator.CalculateMarks(test.Questions, new []{userAnswers});
        
        // Assert
        result[0].Mark.Should().Be(Mark.Five);
    }
    
    [Fact]
    public void CalculateMarks_ShouldThrowInvalidOperationException_WhenQuestionsCountIsNotEqualUserAnswersCount()
    {
        //Arrange
        var test = TestData.GetDefaultTest();
        var calculator = new MarkCalculator();
        var userAnswers = TestingUserAnswersData.GetUserAnswersWithLessCountThenQuestions(test);

        // Assert
        Assert.Throws<InvalidOperationException>(() => calculator.CalculateMarks(test.Questions, new[] {userAnswers}));
    }
}