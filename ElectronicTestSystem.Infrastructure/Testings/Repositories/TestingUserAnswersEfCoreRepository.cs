using System.Linq.Expressions;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ElectronicTestSystem.Infrastructure.Testings.Repositories;

internal sealed class TestingUserAnswersEfCoreRepository : EfCoreRepository<TestingUserAnswers>, ITestingUserAnswersRepository
{
    public TestingUserAnswersEfCoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }


    #region Obsolete

    public Guid Create(TestingUserAnswers testingUserAnswers)
    {
        throw new NotImplementedException();
    }

    #endregion

    public void Update(TestingUserAnswers testingUserAnswers)
    {
        DbContext.Set<TestingUserAnswers>().Update(testingUserAnswers);
    }

    public TestingUserAnswers GetBy(Guid testingId, Guid userId)
    {
        Expression<Func<TestingUserAnswers, bool>> filterByTestingId =
            t => t.TestingId == testingId && t.UserId == userId;
        
        return DbContext.Set<TestingUserAnswers>().First(filterByTestingId);
    }

    public async Task<TestingUserAnswers> GetByAsync(Guid testingId, Guid userId, CancellationToken cancellationToken)
    {
        Expression<Func<TestingUserAnswers, bool>> filterByTestingId =
            t => t.TestingId == testingId && t.UserId == userId;
        
        return await DbContext.Set<TestingUserAnswers>().FirstAsync(filterByTestingId, cancellationToken);
    }

    public async Task<TestingUserAnswers[]> GetByAsync(Guid testingId, CancellationToken cancellationToken)
    {
        Expression<Func<TestingUserAnswers, bool>> filterByTestingId = t => t.TestingId == testingId;
        
        return await DbContext.Set<TestingUserAnswers>()
            .Where(filterByTestingId)
            .ToArrayAsync(cancellationToken);
    }
}