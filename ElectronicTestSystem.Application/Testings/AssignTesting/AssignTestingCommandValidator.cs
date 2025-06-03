using ElectronicTestSystem.Application.Abstractions.Clock;
using FluentValidation;

namespace ElectronicTestSystem.Application.Testings.AssignTesting;

public class AssignTestingCommandValidator : AbstractValidator<AssignTestingCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AssignTestingCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        RuleFor(c => c.AuthorId).NotEmpty();

        RuleFor(c => c.GroupId).NotEmpty();
        
        RuleFor(c => c.EndDate).NotEmpty();
        RuleFor(c => c.EndDate).GreaterThan(_dateTimeProvider.Now);
        
        RuleFor(c => c.TestId).NotEmpty();
    }
}