using ElectronicTestSystem.WebApi.Controllers.Users;

namespace ElectronicTestSystem.WebApi.FunctionalTests.Users;

internal static class UserData
{
    public static RegisterUserRequest RegisterTestUserRequest = new("test@test.com", "test", "test", "12345", true, Guid.Empty);
}
