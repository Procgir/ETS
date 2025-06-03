using System.Net.Http.Json;
using ElectronicTestSystem.Application.Abstractions.Authentication;
using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.Infrastructure.Authentication.Models;

namespace ElectronicTestSystem.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";

    private readonly HttpClient _httpClient;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthenticationService(HttpClient httpClient, IDateTimeProvider dateTimeProvider)
    {
        _httpClient = httpClient;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user, _dateTimeProvider.Now);

        userRepresentationModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(
        HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";

        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header can't be null");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        string userIdentityId = locationHeader.Substring(
            userSegmentValueIndex + usersSegmentName.Length);

        return userIdentityId;
    }
}
