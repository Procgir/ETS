using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;

namespace ElectronicTestSystem.Application.Tests.GetTestWithDapper;

public record GetTestWithDapperQuery(
    Guid TestId,
    Guid UserId) : IQuery<TestResponse>;