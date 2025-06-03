namespace ElectronicTestSystem.Domain.Users;

public interface IUserService
{
    public User Get(Guid userId);
    public Task<Guid> CreateAsync(User user, CancellationToken cancellationToken = default);
}