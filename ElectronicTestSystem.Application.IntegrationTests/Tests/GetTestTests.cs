using ElectronicTestSystem.Application.IntegrationTests.Infrastructure;
using ElectronicTestSystem.Application.Tests.GetTest;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Users;
using FluentAssertions;

namespace ElectronicTestSystem.Application.IntegrationTests.Tests;

public class GetTestTests : BaseIntegrationTest
{
    public GetTestTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    private static readonly Guid TestId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task AssignTesting_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var command = new GetTestQuery(
            TestId,
            UserId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }
}
