using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Application.Testings.GetTestings;

public record GetTestingsQuery(
    Guid UserId,
    GetOptions Options) : IQuery<PagedResultResponse<TestingsItemResponse>>;
