using System.Net;
using System.Net.Http.Json;
using ElectronicTestSystem.WebApi.Controllers.Users;
using ElectronicTestSystem.WebApi.FunctionalTests.Infrastructure;
using FluentAssertions;

namespace ElectronicTestSystem.WebApi.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
    public RegisterUserTests(FunctionalTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Theory]
    [InlineData("", "first", "last", "12345", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test.com", "first", "last", "", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("@test.com", "first", "last", "12345", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@", "first", "last", "12345", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "", "last", "12345", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "", "12345", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "last", "", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "last", "1", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "last", "12", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "last", "123", true, "00000000-0000-0000-0000-000000000000")]
    [InlineData("test@test.com", "first", "last", "1234", true, "00000000-0000-0000-0000-000000000000")]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid(
        string email,
        string firstName,
        string lastName,
        string password,
        bool isTeacher,
        string groupIdString)
    {
        // Arrange
        var request = new RegisterUserRequest(email, firstName, lastName, password, isTeacher, Guid.Parse(groupIdString));

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserRequest("create@test.com", "first", "last", "12345", true, Guid.Empty);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
