using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Tests.DeleteTest;

internal sealed class DeleteTestCommandHandler : ICommandHandler<DeleteTestCommand>
{
    private readonly ITestRepository _testRepository;
    private readonly ITestService _testService;
    private readonly IUserRepository _userRepository;

    public DeleteTestCommandHandler(ITestService testService, IUserRepository userRepository, ITestRepository testRepository)
    {
        _testService = testService;
        _userRepository = userRepository;
        _testRepository = testRepository;
    }

    public async Task<Result> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
    {
        var test = await _testRepository.GetByIdAsync(request.TestId, cancellationToken);

        if (test is null)
        {
            return Result.Failure(TestErrors.NotFound);
        }
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        if (test.AuthorId != user.Id)
        {
            return Result.Failure<Guid>(TestErrors.CantDeleteNotOwnTest);
        }
        
        await _testService.DeleteAsync(test.Id, cancellationToken);
        
        return Result.Success();
    }
}