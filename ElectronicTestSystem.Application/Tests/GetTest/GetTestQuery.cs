using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;

namespace ElectronicTestSystem.Application.Tests.GetTest;

public record GetTestQuery(
    Guid TestId,
    Guid UserId) : IQuery<TestResponse>;