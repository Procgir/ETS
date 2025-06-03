using ElectronicTestSystem.Application.Abstractions.Clock;

namespace ElectronicTestSystem.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}