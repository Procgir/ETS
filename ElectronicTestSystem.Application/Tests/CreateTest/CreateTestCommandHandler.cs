using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Messaging;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Tests.CreateTest;

internal sealed class CreateTestCommandHandler : ICommandHandler<CreateTestCommand, Guid>
{
    private readonly ITestService _testService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTestCommandHandler(ITestService testService, IUserRepository userRepository, IDateTimeProvider dateTimeProvider)
    {
        _testService = testService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateTestCommand request, CancellationToken cancellationToken)
    {
        var author = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author == null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }
        
        var test = Test.Create(
            request.Subject,
            request.Theme,
            author.Id,
            _dateTimeProvider.Now);

        foreach (var question in request.Questions)
        {
            test.AddQuestion(question.Body, question.AnswersOptions, question.TrueAnswerNumber);
        }
        
        await _testService.CreateAsync(test, cancellationToken);
        
        return test.Id;
    }
}