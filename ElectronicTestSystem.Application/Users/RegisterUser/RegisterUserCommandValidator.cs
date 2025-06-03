using FluentValidation;

namespace ElectronicTestSystem.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty();

        RuleFor(c => c.LastName).NotEmpty();

        RuleFor(c => c.Login).EmailAddress();

        RuleFor(c => c.Password).NotEmpty().MinimumLength(5);
    }
}
