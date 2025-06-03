using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Domain.Groups;

public class Group : Entity
{
    public GroupName Name { get; }
    public List<User> Users { get; }
    
    private Group()
    {
        
    }
    
    internal Group(Guid id, GroupName name, List<User> users) : base(id)
    {
        Name = name;
        Users = users;
    }
    
    public static Group Create(string name, List<User> users) 
        => new Group(Guid.NewGuid(), new GroupName(name), users); 
}