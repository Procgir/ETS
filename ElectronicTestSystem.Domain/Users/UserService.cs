using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User Get(Guid userId)
    {
        return _userRepository.Get(userId);
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

}