using System.Net.Http.Json;
using ElectronicTestSystem.ConsoleApi.Clients.Abstractions;
using ElectronicTestSystem.ConsoleApi.Contracts.Marks;

namespace ElectronicTestSystem.ConsoleApi.Clients.Marks;

public class MarksHttpClientApi : HttpClientApiBase, IMarksHttpClientApi
{
    private readonly string CalculateMarksPathTemplate = "{0}/marks/tests/{1}/groups/{2}/calculate";
    public MarksHttpClientApi(string baseUrl, HttpClient httpClient) : base(baseUrl, httpClient)
    {
    }

    public async Task<CalculateMarksApiResponse?> CalculateAsync(string testName, string groupName, 
        CalculateMarksApiRequest request)
    {
        var url = string.Format(CalculateMarksPathTemplate, BaseUrl, testName, groupName);
        var response = await HttpClient.PostAsJsonAsync(url, request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return response.Content.ReadFromJsonAsync<CalculateMarksApiResponse>().Result;
    }
}