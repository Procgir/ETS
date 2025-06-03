using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;

namespace ElectronicTestSystem.Application.Tests.GetTests;

internal sealed class GetTestQueryHandler : IQueryHandler<GetTestsQuery, PagedResultResponse<TestShortResponse>>
{
    private readonly ITestRepository _testRepository;
    private readonly ITestService _testService;

    public GetTestQueryHandler(ITestRepository testRepository, ITestService testService)
    {
        _testRepository = testRepository;
        _testService = testService;
    }

    public async Task<Result<PagedResultResponse<TestShortResponse>>> Handle(GetTestsQuery request, CancellationToken cancellationToken)
    {
        var testsResult = await _testRepository.GetAsync(request.UserId, request.GetOptions, cancellationToken);

        return testsResult.ToResponse(t => t.ToResponseShort());
    }
}