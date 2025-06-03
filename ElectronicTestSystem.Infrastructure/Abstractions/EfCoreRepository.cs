using System.Linq.Expressions;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.SDK.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectronicTestSystem.Infrastructure.Abstractions;

internal abstract class EfCoreRepository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected EfCoreRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await DbContext
            .Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual void Add(T entity)
    {
        DbContext.Add(entity);
    }

    public async Task<PaginatedResult<T>> GetPagedAsync(GetOptions options,
        CancellationToken cancellationToken = default)
    {
        var totalCount = DbContext.Set<T>().Count();
        var (pageNumber, pageSize) = GetPagedOptions(options);
        
        var items = await GetPagedQuery(pageNumber, pageSize)
            .ToArrayAsync(cancellationToken);

        return new PaginatedResult<T>(items, pageNumber, pageSize, totalCount);
    }

    protected async Task<PaginatedResult<T>> GetPagedAsync(Expression<Func<T, bool>> filterBy, GetOptions options,
        CancellationToken cancellationToken = default)
    {
        var totalCount = DbContext.Set<T>().Where(filterBy).Count();
        var (pageNumber, pageSize) = GetPagedOptions(options);
        
        var items = await GetPagedQuery(pageNumber, pageSize, filterBy)
            .ToArrayAsync(cancellationToken);
        
        return new PaginatedResult<T>(items, pageNumber, pageSize, totalCount);
    }

    private (int pageNumber, int pageSize) GetPagedOptions(GetOptions options)
    {
        var pageSize = options.Get<int>(CommonGetOptionsKeys.PageSize);
        var pageNumber = options.Get<int>(CommonGetOptionsKeys.PageNumber);

        return (pageNumber, pageSize);
    }

    private IQueryable<T> GetPagedQuery(int pageNumber, int pageSize)
    {
        return DbContext
            .Set<T>()
            .OrderByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
    
    private IQueryable<T> GetPagedQuery(int pageNumber, int pageSize, Expression<Func<T, bool>> filterBy)
    {
        return DbContext
            .Set<T>()
            .Where(filterBy)
            .OrderByDescending(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}