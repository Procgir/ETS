using ElectronicTestSystem.Application.Abstractions.Messaging;

namespace ElectronicTestSystem.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;
