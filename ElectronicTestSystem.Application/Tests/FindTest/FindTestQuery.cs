using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Tests.FindTest;

public record FindTestQuery(
    string Query,
    Guid UserId) : IQuery<TestsResponse>;