using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Tests.DeleteTest;

public record DeleteTestCommand(
    Guid TestId,
    Guid UserId) : ICommand;