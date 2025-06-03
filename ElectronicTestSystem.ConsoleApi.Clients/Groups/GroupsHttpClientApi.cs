using System.Net.Http.Json;
using ElectronicTestSystem.ConsoleApi.Clients.Abstractions;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;

namespace ElectronicTestSystem.ConsoleApi.Clients.Groups;

public class GroupsHttpClientApi : HttpClientApiBase, IGroupsHttpClientApi
{
    private readonly string GroupPath = "groups";
    public GroupsHttpClientApi(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
    {
    }

    public async Task<CreateGroupApiResponse?> CreateAsync(CreateGroupApiRequest request)
    {
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{GroupPath}", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return response.Content.ReadFromJsonAsync<CreateGroupApiResponse>().Result;
    }

    public async Task<GetGroupsApiResponse?> GetAsync()
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{GroupPath}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return response.Content.ReadFromJsonAsync<GetGroupsApiResponse>().Result;
    }
}