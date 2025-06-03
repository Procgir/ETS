using ElectronicTestSystem.Application.Abstractions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Tests.Events;
using ElectronicTestSystem.Domain.Users;
using MediatR;

namespace ElectronicTestSystem.Application.Tests.CreateTest;

internal class TestCreatedDomainEventHandler : INotificationHandler<TestCreatedDomainEvent>
{
    private readonly ITestRepository _testRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public TestCreatedDomainEventHandler(ITestRepository testRepository, IUserRepository userRepository, IEmailService emailService)
    {
        _testRepository = testRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(TestCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var test = await _testRepository.GetByIdAsync(notification.TestId, cancellationToken);

        if (test is null)
        {
            return;
        }
        
        var user = await _userRepository.GetByIdAsync(test.AuthorId, cancellationToken);

        if (user is null)
        {
            return;
        }

        var email = new Email(user.Login.Value);

        await _emailService.SendAsync(email, $"Test {test.Id} created successfully!", $"Test has Subject: {test.Subject}, Theme: {test.Theme}");
    }
}