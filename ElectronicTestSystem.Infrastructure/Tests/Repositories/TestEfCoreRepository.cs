using System.Linq.Expressions;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure.Abstractions;
using ElectronicTestSystem.SDK.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectronicTestSystem.Infrastructure.Tests.Repositories;

internal sealed class TestEfCoreRepository : EfCoreRepository<Test>, ITestRepository
{
    public TestEfCoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    #region Obsolete

    public Dictionary<string, Test> GetDictItems()
    {
        throw new NotImplementedException();
    }

    public void Save(Test test)
    {
        throw new NotImplementedException();
    }

    public void SaveDictItems(Dictionary<string, Test> dictItems)
    {
        throw new NotImplementedException();
    }

    public List<Test> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Create(Test test)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Test test)
    {
        throw new NotImplementedException();
    }
    
    public Task RemoveAsync(Guid testId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    #endregion

    public Test Get(Guid testId)
    {
        return DbContext.Set<Test>().First(t=> t.Id == testId);
    }

    public PaginatedResult<Test> Get(GetOptions options)
    {
        return GetPagedAsync(options, default).Result;
    }

    public Task<PaginatedResult<Test>> GetAsync(Guid userId, GetOptions options, CancellationToken cancellationToken)
    {
        Expression<Func<Test, bool>> filterByUserId = t => t.AuthorId == userId;
        
        return GetPagedAsync(filterByUserId, options, cancellationToken);
    }

    public async Task<Test[]> GetByAsync(Guid[] testsId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Test>()
            .Where(t => testsId.Contains(t.Id))
            .ToArrayAsync(cancellationToken);
    }

    public void Delete(Guid testId)
    {
        var test = Get(testId);
        
        DbContext.Set<Test>().Remove(test);
    }

    public void Update(Test test)
    {
        DbContext.Set<Test>().Update(test);
    }
}