namespace ElectronicTestSystem.Application.Users.GetLoggedInUser;

public sealed class UserResponse
{
    public Guid Id { get; init; }

    public bool IsTeacher { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}
