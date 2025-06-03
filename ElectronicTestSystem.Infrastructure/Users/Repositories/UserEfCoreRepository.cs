using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.Infrastructure.Abstractions;

namespace ElectronicTestSystem.Infrastructure.Users.Repositories;

internal sealed class UserEfCoreRepository : EfCoreRepository<User>, IUserRepository
{
    public UserEfCoreRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        
    }

    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }

    public User Get(Guid userId)
    {
        return DbContext
            .Set<User>()
            .First(u => u.Id == userId);
    }
}