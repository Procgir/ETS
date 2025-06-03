using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ElectronicTestSystem.Infrastructure.Groups.Repositories;

internal sealed class GroupEfCoreRepository : EfCoreRepository<Group>, IGroupRepository
{
    public GroupEfCoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Group[] Get()
    {
        return DbContext.Set<Group>().ToArray();
    }

    public Group Get(Guid groupId)
    {
        return DbContext.Set<Group>().First(g => g.Id == groupId);
    }

    public Group? FindByUserId(Guid userId)
    {
        return DbContext.Set<Group>()
            .Include(g => g.Users)
            .FirstOrDefault(g => g.Users.Any(u => u.Id == userId)); 
    }
    
    #region Obsolete

    public Dictionary<string, Group> GetDictItems()
    {
        throw new NotImplementedException();
    }

    public List<Group> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Save(Group test)
    {
        throw new NotImplementedException();
    }

    public void SaveDictItems(Dictionary<string, Group> dictItems)
    {
        throw new NotImplementedException();
    }

    #endregion
}