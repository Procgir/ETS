using System.Net.Http.Json;
using ElectronicTestSystem.ConsoleApi.Clients.Tests;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;

namespace ElectronicTestSystem.ConsoleApi.Clients.Abstractions.Tests;

public class TestsHttpClientApi : HttpClientApiBase, ITestsHttpClientApi
{
    private readonly string TestsPath = "tests";
    public TestsHttpClientApi(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
    {
    }


    public async Task<CreateTestApiResponse?> CreateAsync(CreateTestApiRequest request)
    {
        var response = await HttpClient.PostAsJsonAsync($"{BaseUrl}/{TestsPath}", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return response.Content.ReadFromJsonAsync<CreateTestApiResponse>().Result;
    }

    public async Task<GetTestsApiResponse?> GetAsync()
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}/{TestsPath}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return response.Content.ReadFromJsonAsync<GetTestsApiResponse>().Result;
    }
}