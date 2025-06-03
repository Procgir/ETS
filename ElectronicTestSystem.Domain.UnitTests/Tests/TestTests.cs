using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Tests.Events;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.DomainUnitTests.Infrastructure;
using ElectronicTestSystem.DomainUnitTests.Users;
using FluentAssertions;

namespace ElectronicTestSystem.DomainUnitTests.Tests;

public class TestTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValue()
    {
        // Act
        var test = Test
            .Create(TestData.SubjectName,
                TestData.ThemeName,
                TestData.AuthorId,
                TestData.CreatedAt);
        
        // Assert
        test.AuthorId.Should().Be(TestData.AuthorId);
        test.Subject.Name.Should().Be(TestData.SubjectName);
        test.Theme.Name.Should().Be(TestData.ThemeName);
        test.CreatedAt.Should().Be(TestData.CreatedAt);
    }
    
    [Fact]
    public void Create_Should_RaiseTestCreatedDomainEvent()
    {
        // Act
        var test = Test
            .Create(TestData.SubjectName,
                TestData.ThemeName,
                TestData.AuthorId,
                TestData.CreatedAt);

        // Assert
        var testCreatedDomaintEvent = AssertDomainEventWasPublished<TestCreatedDomainEvent>(test);

        testCreatedDomaintEvent.TestId.Should().Be(test.Id);
    }
    
    [Fact]
    public void AddQuestions_Should_SetQuestions()
    {
        // Act
        var test = Test
            .Create(TestData.SubjectName,
                TestData.ThemeName,
                TestData.AuthorId,
                TestData.CreatedAt);
        foreach (var questionBodyDataPair in TestData.Questions)
        {
            var questionBody = questionBodyDataPair.Key;
            var questionInfo = questionBodyDataPair.Value;
            test.AddQuestion(questionBody, questionInfo.AnswersOptions, questionInfo.TrueAnswerNumber);
        }

        // Assert
        test.Questions.Count.Should().Be(TestData.Questions.Count);
    }
}
