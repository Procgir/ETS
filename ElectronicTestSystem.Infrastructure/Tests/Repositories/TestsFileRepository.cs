using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure.Abstractions;
using ElectronicTestSystem.SDK.Converters;
using ElectronicTestSystem.SDK.Data;

namespace ElectronicTestSystem.Infrastructure.Tests.Repositories;

public class TestsFileRepository(string fileName, Converter<Test> converter)
    : FileRepository<Test>(fileName, converter), ITestRepository
{
    public void Create(Test test)
    {
        throw new NotImplementedException();
    }

    public void Add(Test test)
    {
        throw new NotImplementedException();
    }

    public Test Get(Guid testId)
    {
        throw new NotImplementedException();
    }

    public Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Guid testId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedResult<Test>> GetAsync(Guid userId, GetOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Test[]> GetByAsync(Guid[] testsId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid testId)
    {
        throw new NotImplementedException();
    }

    public void Update(Test test)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Test test)
    {
        throw new NotImplementedException();
    }

    public PaginatedResult<Test> Get(GetOptions options)
    {
        throw new NotImplementedException();
    }
}