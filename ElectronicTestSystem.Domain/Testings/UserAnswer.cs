using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Domain.Testings;

public record UserAnswer(Guid QuestionId,
    TestQuestionAnswerNumber AnswerNumber);