using ElectronicTestSystem.Domain.Testings.Calculators;

namespace ElectronicTestSystem.Application.Dto;

public class UserMarkDto(string userName, int mark)
{
    public string UserName { get; } = userName;
    public int Mark { get; } = mark;
    internal static UserMarkDto From(string userName, Mark mark) => new UserMarkDto(userName, (int)mark.Value);
}