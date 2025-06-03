using ElectronicTestSystem.Domain.Groups;

namespace ElectronicTestSystem.Application.Dto;

public class GroupDto(string name, List<string> usersNames)
{
    public string Name { get; } = name;
    public List<string> UsersNames { get; } = usersNames;

    internal static GroupDto From(Group group)
    {
        return new GroupDto(group.Name.Value, 
            group.Users.Select(u => u.Name.FirstName).ToList());
    }
    
    public static GroupDto Empty => new GroupDto(string.Empty, new List<string>());
}
