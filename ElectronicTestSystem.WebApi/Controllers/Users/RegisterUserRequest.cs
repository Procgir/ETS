namespace ElectronicTestSystem.WebApi.Controllers.Users;

public sealed record RegisterUserRequest(
    string Login,
    string FirstName,
    string LastName,
    string Password,
    bool IsTeacher,
    Guid GroupId);
