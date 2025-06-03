using ElectronicTestSystem.Application.Abstractions.Behaviors;

namespace ElectronicTestSystem.Application.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }
    
    public IEnumerable<ValidationError> Errors { get; }
}