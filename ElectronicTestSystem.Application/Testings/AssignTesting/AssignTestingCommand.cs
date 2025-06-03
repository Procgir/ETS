using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Testings.AssignTesting;

public record AssignTestingCommand(
    Guid TestId,
    Guid GroupId,
    Guid AuthorId,
    DateTime EndDate) : ICommand<AssignTestingResponse>;