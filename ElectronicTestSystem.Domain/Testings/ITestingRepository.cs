using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.SDK.Data;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Testings;

public interface ITestingRepository
{
    public void Add(Testing test);
    public Testing Get(Guid testingId);
    public Task<Testing?> GetAsync(Guid testingId, CancellationToken cancellationToken);
    public Task<PaginatedResult<Testing>> GetPagedByAuthorAsync(Guid authorId, GetOptions getOptions, CancellationToken cancellationToken);
    public PaginatedResult<Testing> Get(GetOptions getOptions);
    public Testing[]? Find(string query);
    
    
    public void Update(Testing testing);
    public void Delete(Guid testingId);
    
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void Create(Testing testing);
}