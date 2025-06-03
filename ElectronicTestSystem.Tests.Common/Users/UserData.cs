using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Tests.Common.Users;

internal static class UserData
{
    public static readonly UserName Name = new("First", string.Empty, "Last");
    public static readonly UserLogin Login = new("test@test.com");
    public static readonly UserPassword Password = new("password_hash");
}
