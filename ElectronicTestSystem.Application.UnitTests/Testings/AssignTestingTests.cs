using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Testings.AssignTesting;
using ElectronicTestSystem.Application.UnitTests.Users;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.Tests.Common.Groups;
using FluentAssertions;
using NSubstitute;
using ElectronicTestSystem.Tests.Common.Tests;

namespace ElectronicTestSystem.Application.UnitTests.Testings;

public class AssignTestingTests
{
    private readonly ITestingRepository _testingRepositoryMock;
    private readonly ITestingUserAnswersRepository _testingUserAnswersRepositoryMock;
    private readonly ITestingService _testingServiceMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IGroupService _groupServiceMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly ITestService _testServiceMock;
    private readonly AssignTestingCommandHandler _handler;

    private static readonly DateTime Now = DateTime.Now;
    private static readonly AssignTestingCommand Command = new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        DateTime.Now.AddSeconds(60));

    public AssignTestingTests()
    {
        _testingRepositoryMock = Substitute.For<ITestingRepository>();
        _testingUserAnswersRepositoryMock = Substitute.For<ITestingUserAnswersRepository>();
        _testingServiceMock = Substitute.For<ITestingService>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _groupServiceMock = Substitute.For<IGroupService>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _testServiceMock = Substitute.For<ITestService>();
        
        _dateTimeProviderMock.Now.Returns(Now);

        _handler = new AssignTestingCommandHandler(
            _testingServiceMock,
            _userRepositoryMock,
            _testingRepositoryMock,
            _dateTimeProviderMock,
            _groupServiceMock,
            _testingUserAnswersRepositoryMock,
            _testServiceMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNull()
    {
        // Arrange
        _userRepositoryMock
            .GetByIdAsync(Command.AuthorId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        Result<AssignTestingResponse> result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShoulReturnSuccess_WhenTestingIsAssigned()
    {
        // Arrange
        var group = GroupData.Default;
        var user = UserData.DefaultTeacher;
        var test = TestData.GetDefaultTest(user.Id);
        _userRepositoryMock
            .GetByIdAsync(Command.AuthorId, Arg.Any<CancellationToken>())
            .Returns(user);

        _testServiceMock
            .Get(test.Id)
            .Returns(test);

        _groupServiceMock
            .GetByAsync(group.Id, Arg.Any<CancellationToken>())
            .Returns(group);
        
        // Act
        Result<AssignTestingResponse> result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task Handle_ShouldCallRepository_WhenTestingIsAssigned()
    {
        // Arrange
        var group = GroupData.Default;
        var user = UserData.DefaultTeacher;
        var test = TestData.GetDefaultTest(user.Id);
        _userRepositoryMock
            .GetByIdAsync(Command.AuthorId, Arg.Any<CancellationToken>())
            .Returns(user);

        _testServiceMock
            .Get(test.Id)
            .Returns(test);

        _groupServiceMock
            .GetByAsync(group.Id, Arg.Any<CancellationToken>())
            .Returns(group);
    
        // Act
        Result<AssignTestingResponse> result = await _handler.Handle(Command, default);

        // Assert
        _testingRepositoryMock.Received(1).Add(Arg.Is<Testing>(t => t.Id == result.Value.TestingId));
    }
}
