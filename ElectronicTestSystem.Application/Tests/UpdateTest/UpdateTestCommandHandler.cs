using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Application.Tests.CreateTest;
using ElectronicTestSystem.Application.Tests.GetTest;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Tests.UpdateTest;

internal sealed class UpdateTestCommandHandler : ICommandHandler<UpdateTestCommand>
{
    private readonly ITestService _testService;
    private readonly IUserRepository _userRepository;
    private readonly ITestRepository _testRepository;

    public UpdateTestCommandHandler(ITestService testService, IUserRepository userRepository, ITestRepository testRepository)
    {
        _testService = testService;
        _userRepository = userRepository;
        _testRepository = testRepository;
    }

    public async Task<Result> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
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
        
        var subject = _testService.CreateSubject(request.Subject);
        var theme = _testService.CreateTheme(request.Theme);
        var questions =  request
            .Questions
            .Select(q => _testService.CreateQuestion(test.Id, q.Body, q.AnswersOptions, q.TrueAnswerNumber))
            .ToArray();
        test.Update(subject,
            theme,
            questions);
        
        await _testService.UpdateAsync(test, cancellationToken);
        
        return Result.Success();
    }
}