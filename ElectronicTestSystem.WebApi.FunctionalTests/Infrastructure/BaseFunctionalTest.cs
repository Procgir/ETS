using System.Net.Http.Json;
using ElectronicTestSystem.Application.Users.LogInUser;
using ElectronicTestSystem.WebApi.Controllers.Users;
using ElectronicTestSystem.WebApi.FunctionalTests.Users;

namespace ElectronicTestSystem.WebApi.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected async Task<string> GetAccessToken()
    {
        HttpResponseMessage loginResponse = await HttpClient.PostAsJsonAsync(
            "api/v1/users/login",
            new LogInUserRequest(
                UserData.RegisterTestUserRequest.Login,
                UserData.RegisterTestUserRequest.Password));

        AccessTokenResponse? accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return accessTokenResponse!.AccessToken;
    }
}
