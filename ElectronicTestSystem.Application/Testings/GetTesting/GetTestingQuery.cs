using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Testings.GetTesting;

public record GetTestingQuery(
    Guid TestingId,
    Guid UserId) : IQuery<TestingResponse>;
