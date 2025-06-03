using System.Linq.Expressions;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Infrastructure.Abstractions;
using ElectronicTestSystem.SDK.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectronicTestSystem.Infrastructure.Testings.Repositories;

internal sealed class TestingEfCoreRepository : EfCoreRepository<Testing>, ITestingRepository
{
    public TestingEfCoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public void Add(Test test)
    {
        DbContext.Set<Test>().Add(test);
    }

    public Testing Get(Guid testingId)
    {
        return DbContext.Set<Testing>().First(t => t.Id == testingId);
    }

    public Task<Testing?> GetAsync(Guid testingId, CancellationToken cancellationToken)
    {
        return GetByIdAsync(testingId, cancellationToken);
    }

    public Task<PaginatedResult<Testing>> GetPagedByAuthorAsync(Guid authorId, GetOptions getOptions, CancellationToken cancellationToken)
    {
        Expression<Func<Testing, bool>> filterByAuthorId = t => t.AuthorId == authorId;
        
        return GetPagedAsync(filterByAuthorId, getOptions, cancellationToken);
    }

    public PaginatedResult<Testing> Get(GetOptions getOptions)
    {
        return GetPagedAsync(getOptions, default).Result;
    }

    
    public Testing[]? Find(string query)
    {
        return DbContext.Set<Testing>()
            .AsNoTracking()
            .Include(t => t.Test)
            .Where(t => 
                EF.Functions.Like(t.Test.Subject.Name, $"%{query}%") 
                || EF.Functions.Like(t.Test.Theme.Name, $"%{query}%"))
            .ToArray();
    }
    

    public void Update(Testing testing)
    {
        DbContext.Set<Testing>().Update(testing);
    }

    public void Delete(Guid testingId)
    {
        var testing = Get(testingId);
        
        DbContext.Set<Testing>().RemoveRange(testing);
    }



    #region Obsolete

    public void Create(Testing testing)
    {
        throw new NotImplementedException();
    }

    #endregion
}