using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Dto;

public class UserDto(string name)
{
    public string Name { get; } = name;
    
    public override string ToString()
    {
        return Name;
    }
    
    internal static UserDto From(User user) => new UserDto(user.Name.FirstName);
}