using ElectronicTestSystem.ConsoleApi.Contracts.Marks;

namespace ElectronicTestSystem.ConsoleApi.Clients.Marks;


public interface IMarksHttpClientApi
{
    public Task<CalculateMarksApiResponse?> CalculateAsync(string testName, string groupName, CalculateMarksApiRequest request); 
}