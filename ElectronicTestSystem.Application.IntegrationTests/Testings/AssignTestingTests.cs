using ElectronicTestSystem.Application.IntegrationTests.Infrastructure;
using ElectronicTestSystem.Application.Testings.AssignTesting;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Users;
using FluentAssertions;

namespace ElectronicTestSystem.Application.IntegrationTests.Testings;

public class AssignTestingTests : BaseIntegrationTest
{
    private static readonly Guid TestId = Guid.NewGuid();
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid AuthorId = Guid.NewGuid();
    private static readonly DateTime EndDate = DateTime.Now.AddDays(1);

    public AssignTestingTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AssignTesting_ShouldReturnFailure_WhenAuthorIsNotFound()
    {
        // Arrange
        var command = new AssignTestingCommand(
            TestId,
            GroupId,
            AuthorId,
            EndDate);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }
}
