using ElectronicTestSystem.SDK.Data;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Tests;

public interface ITestRepository
{
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Dictionary<string, Test> GetDictItems();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void Save(Test test);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void SaveDictItems(Dictionary<string, Test> dictItems);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public List<Test> GetAll();
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public void Create(Test test);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Task UpdateAsync(Test test);
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public Task RemoveAsync(Guid testId, CancellationToken cancellationToken);
    
    public void Add(Test test);
    public Test Get(Guid testId);
    public Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken);
    public PaginatedResult<Test> Get(GetOptions options);
    public Task<PaginatedResult<Test>> GetAsync(Guid userId, GetOptions options, CancellationToken cancellationToken);
    public Task<Test[]> GetByAsync(Guid[] testsId, CancellationToken cancellationToken);
    public void Delete(Guid testId);
    public void Update(Test test);
}