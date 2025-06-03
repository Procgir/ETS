using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
        string Login,
        string FirstName,
        string LastName,
        string Password,
        bool IsTeacher,
        Guid GroupId) : ICommand<Guid>;
