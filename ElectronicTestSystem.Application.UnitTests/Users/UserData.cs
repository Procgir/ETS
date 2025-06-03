using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.UnitTests.Users;

internal static class UserData
{
    public static readonly UserName Name = new("First", string.Empty, "Last");
    public static readonly UserLogin Login = new("test@test.com");
    public static readonly UserPassword Password = new("password_hash");
    
    public static User DefaultTeacher => 
        User.Create(Name, Login, Password, true);
}
