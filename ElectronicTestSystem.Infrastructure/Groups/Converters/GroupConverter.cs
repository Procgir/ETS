using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.SDK.Converters;

namespace ElectronicTestSystem.Infrastructure.Groups.Converters;

public class GroupConverter : Converter<Group>
{
    public override string DictKey(Group item) => item.Name.Value;

    public override string Serialize(Group group)
    {
        return $"{group.Name}{SeparatorSymbol}{string.Join(SeparatorSymbol, group.Users.Select(u => u.Name))}";
    }
    
    public override Group Deserialize(string rawTextGroup)
    {
        string[] rawTextUserNames = rawTextGroup.Split(SeparatorSymbol);
        
        string groupName = rawTextUserNames[0];
        List<User> users = rawTextUserNames
            .Skip(1)
            .Select(User.Create)
            .ToList();

        return Group.Create(groupName, users);
    }
}