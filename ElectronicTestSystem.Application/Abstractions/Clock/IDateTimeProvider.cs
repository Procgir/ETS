namespace ElectronicTestSystem.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
}