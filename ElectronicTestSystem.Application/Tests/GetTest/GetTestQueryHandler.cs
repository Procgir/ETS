using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Dto;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Tests.GetTest;

internal sealed class GetTestQueryHandler : IQueryHandler<GetTestQuery, TestResponse>
{
    private readonly ITestRepository _testRepository;
    private readonly IUserRepository _userRepository;

    public GetTestQueryHandler(ITestRepository testRepository, IUserRepository userRepository)
    {
        _testRepository = testRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<TestResponse>> Handle(GetTestQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<TestResponse>(UserErrors.NotFound);
        }

        var test = await _testRepository.GetByIdAsync(request.TestId, cancellationToken);

        if (test is null)
        {
            return Result.Failure<TestResponse>(TestErrors.NotFound);
        }

        if (test.AuthorId != user.Id)
        {
            return Result.Failure<TestResponse>(TestErrors.CantGetNotOwnTest);
        }
        
        return test.ToResponse();
    }
}