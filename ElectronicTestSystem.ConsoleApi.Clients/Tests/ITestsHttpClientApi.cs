using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Tests.Get;

namespace ElectronicTestSystem.ConsoleApi.Clients.Tests;


public interface ITestsHttpClientApi
{
    public Task<CreateTestApiResponse?> CreateAsync(CreateTestApiRequest request); 
    public Task<GetTestsApiResponse?> GetAsync(); 
}