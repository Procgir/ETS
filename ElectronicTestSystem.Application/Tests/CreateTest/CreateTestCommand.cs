using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Tests.CreateTest;

public record CreateTestCommand(
    string Subject,
    string Theme,
    CreateTestCommandQuestion[] Questions,
    Guid AuthorId) :  ICommand<Guid>;