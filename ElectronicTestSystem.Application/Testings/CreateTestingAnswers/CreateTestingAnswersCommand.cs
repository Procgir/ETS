using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Testings.AssignTesting;

namespace ElectronicTestSystem.Application.Testings.CreateTestingAnswers;

public record CreateTestingAnswersCommand(
    Guid UserId,
    Guid TestingId,
    int[] UserAnswers) : ICommand;

