namespace ElectronicTestSystem.Domain.Users;

public interface IUserRepository
{
    public User Get(Guid userId);
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(User user);
}