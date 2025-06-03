using ElectronicTestSystem.Application.Testings.GetTestings;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;

namespace ElectronicTestSystem.Application.Testings.SearchTestings;

public record SearchTestingsQuery(string Query) : IQuery<PagedResultResponse<TestingsItemResponse>>;