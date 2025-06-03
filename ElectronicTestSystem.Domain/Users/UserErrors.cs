using ElectronicTestSystem.Domain.Abstractions;

namespace ElectronicTestSystem.Domain.Users;

public static class UserErrors
{
    public static Error NotFound =
        new("User.NotFound",
            "User not found.");
    
    public static Error InvalidCredentials =
        new("User.InvalidCredentials",
            "The provided credentials were invalid");
}