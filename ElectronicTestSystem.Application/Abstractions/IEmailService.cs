using ElectronicTestSystem.Domain.Users;

namespace ElectronicTestSystem.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(Email recipient, string subject, string body);
}