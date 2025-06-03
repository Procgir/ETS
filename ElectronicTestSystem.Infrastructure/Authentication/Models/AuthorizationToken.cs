using System.Text.Json.Serialization;

namespace ElectronicTestSystem.Infrastructure.Authentication.Models;

internal sealed class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
}
