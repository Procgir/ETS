using System.Net;
using System.Net.Http.Json;
using ElectronicTestSystem.WebApi.Controllers.Users;
using ElectronicTestSystem.WebApi.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace ElectronicTestSystem.WebApi.FunctionalTests.Users;

public class LoginUserTests : BaseFunctionalTest
{
    private const string Login = "login@test.com";
    private const string Password = "12345";

    public LoginUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new LogInUserRequest(Login, Password);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest(Login, "first", "last", Password, false, Guid.Empty);
        await HttpClient.PostAsJsonAsync("api/v1/users/register", registerRequest);

        var request = new LogInUserRequest(Login, Password);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
