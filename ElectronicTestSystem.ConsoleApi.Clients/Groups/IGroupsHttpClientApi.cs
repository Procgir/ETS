using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Create;
using ElectronicTestSystem.ConsoleApi.Contracts.Groups.Get;

namespace ElectronicTestSystem.ConsoleApi.Clients.Groups;


public interface IGroupsHttpClientApi
{
    public Task<CreateGroupApiResponse?> CreateAsync(CreateGroupApiRequest request); 
    public Task<GetGroupsApiResponse?> GetAsync(); 
}