using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.DomainUnitTests.Infrastructure;
using FluentAssertions;

namespace ElectronicTestSystem.DomainUnitTests.Users;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValue()
    {
        // Act
        var user = User.Create(UserData.Name, UserData.Login, UserData.Password);

        // Assert
        user.Name.Should().Be(UserData.Name);
        user.Login.Should().Be(UserData.Login);
        user.Password.Should().Be(UserData.Password);
    }

    [Fact]
    public void Create_Should_AddRegisteredRoleToUser()
    {
        // Act
        var user = User.Create(UserData.Name, UserData.Login, UserData.Password);

        // Assert
        user.Roles.Should().Contain(Role.Registered);
    }
}
