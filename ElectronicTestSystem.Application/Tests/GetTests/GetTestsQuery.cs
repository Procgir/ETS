using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.Tests.GetTests;

public record GetTestsQuery(
    Guid UserId,
    GetOptions GetOptions) : IQuery<PagedResultResponse<TestShortResponse>>;