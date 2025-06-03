using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.SDK.Shared;

namespace ElectronicTestSystem.Domain.Users;

public class User : Entity
{
    public UserName Name { get; }
    public bool IsTeacher { get; }
    public UserLogin Login { get; }
    public UserPassword Password { get; }
    public string IdentityId { get; private set; }
    
    public ICollection<Group> Groups { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();
    
    private readonly List<Role> _roles = new ();
    
    private User(){}
    
    internal User(Guid id, UserName name, UserLogin login, UserPassword password, bool isTeacher = false) : base(id)
    {
        Name = name;
        Login = login;
        Password = password;
        IsTeacher = isTeacher;
    }

    public static User Create(
        UserName userName,
        UserLogin login,
        UserPassword password,
        bool isTeacher = false)
    {
        var user = new User(Guid.NewGuid(), userName, login, password, isTeacher);
        user._roles.Add(Role.Registered);

        return user;
    }
    
    [Obsolete(ObsoleteMessages.UIOldConsoleObsoleteMethods)]
    public static User Create(string name) 
        => new User(
            Guid.NewGuid(),
            new UserName(name, string.Empty, string.Empty),
            new UserLogin(String.Empty),
            new UserPassword(String.Empty));

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}